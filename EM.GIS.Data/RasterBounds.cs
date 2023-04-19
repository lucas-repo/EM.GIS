using EM.GIS.Geometries;
using System;
using System.ComponentModel;
using System.IO;
namespace EM.GIS.Data
{
    /// <summary>
    /// 栅格范围
    /// </summary>
    public class RasterBounds : IRasterBounds
    {
        #region Fields

        private readonly int _numColumns;
        private readonly int _numRows;

        private double[] _affine;
        private string _worldFile=string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="RasterBounds"/> 类.
        /// </summary>
        public RasterBounds()
        {
            _affine = new double[6];
        }

        /// <summary>
        /// 初始化 <see cref="RasterBounds"/> 类.
        /// </summary>
        /// <param name="numRows">行数</param>
        /// <param name="numColumns">列数</param>
        /// <param name="worldFileName">世界文件路径</param>
        public RasterBounds(int numRows, int numColumns, string worldFileName)
        {
            _numRows = numRows;
            _numColumns = numColumns;
            _affine = new double[6];
            OpenWorldFile(worldFileName);
        }

        /// <summary>
        /// 初始化 <see cref="RasterBounds"/> 类.
        /// </summary>
        /// <param name="numRows">行数</param>
        /// <param name="numColumns">列数</param>
        /// <param name="affineCoefficients">仿射六参数</param>
        public RasterBounds(int numRows, int numColumns, double[] affineCoefficients)
        {
            _affine = affineCoefficients;
            _numRows = numRows;
            _numColumns = numColumns;
        }

        /// <summary>
        /// 初始化 <see cref="RasterBounds"/> 类.
        /// </summary>
        /// <param name="numRows">行数</param>
        /// <param name="numColumns">列数</param>
        /// <param name="bounds">范围</param>
        public RasterBounds(int numRows, int numColumns, IExtent bounds)
        {
            _affine = new double[6];
            _numRows = numRows;
            _numColumns = numColumns;
            Extent = bounds;
        }

        #endregion

        #region Properties
        /// <inheritdoc/>
        public virtual double[] AffineCoefficients
        {
            get
            {
                return _affine;
            }

            set
            {
                _affine = value;
            }
        }

        /// <summary>
        /// Gets or sets the desired height per cell. This will keep the skew the same, but
        /// will adjust both the column based and row based height coefficients in order
        /// to match the specified cell height. This can be thought of as the height
        /// of a bounding box that contains an entire grid cell, no matter if it is skewed.
        /// </summary>
        public double CellHeight
        {
            get
            {
                double[] affine = AffineCoefficients;

                // whatever sign the coefficients are, they only increase the cell hight
                return Math.Abs(affine[4]) + Math.Abs(affine[5]);
            }

            set
            {
                double[] affine = AffineCoefficients;
                double columnFactor = affine[4] / CellWidth;
                double rowFactor = affine[5] / CellWidth;
                affine[4] = Math.Sign(affine[4]) * value * columnFactor;
                affine[5] = Math.Sign(affine[5]) * value * rowFactor;
                AffineCoefficients = affine; // use the setter for overriding classes
            }
        }
        /// <inheritdoc/>
        public double CellWidth
        {
            get
            {
                double[] affine = AffineCoefficients;

                // whatever sign the coefficients are, they only increase the cell width
                return Math.Abs(affine[1]) + Math.Abs(affine[2]);
            }

            set
            {
                double[] affine = AffineCoefficients;
                double columnFactor = affine[1] / CellWidth;
                double rowFactor = affine[2] / CellWidth;
                affine[1] = Math.Sign(affine[1]) * value * columnFactor;
                affine[2] = Math.Sign(affine[2]) * value * rowFactor;
                AffineCoefficients = affine; // use the setter for overriding classes
            }
        }

        /// <inheritdoc/>
        public IExtent Extent
        {
            get
            {
                double[] affine = AffineCoefficients;
                if (affine[1] == 0 || affine[5] == 0) return null;

                return new Extent(Left, Bottom, Right, Top);
            }

            set
            {
                // Preserve the skew, but translate and scale to fit the envelope.
                if (value != null)
                {
                    X = value.X;
                    Y = value.Y;
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }

        /// <inheritdoc/>
        public double Height
        {
            get
            {
                return (Math.Abs(_affine[4]) * NumColumns) + (Math.Abs(_affine[5]) * NumRows);
            }

            set
            {
                if (Height == 0 && _numRows > 0)
                {
                    _affine[5] = -(value / _numRows);
                    _affine[4] = 0;
                    return;
                }

                double columnFactor = NumColumns * Math.Abs(_affine[4]) / Height;
                double rowFactor = NumRows * Math.Abs(_affine[5]) / Height;
                double newColumnHeight = value * columnFactor;
                double newRowHeight = value * rowFactor;
                _affine[4] = Math.Sign(_affine[4]) * newColumnHeight / NumColumns;
                _affine[5] = Math.Sign(_affine[5]) * newRowHeight / NumRows;
            }
        }

        /// <inheritdoc/>
        public virtual int NumColumns => _numColumns;
        /// <inheritdoc/>
        public virtual int NumRows => _numRows;

        /// <inheritdoc/>
        public double Width
        {
            get
            {
                return (NumColumns * Math.Abs(_affine[1])) + (NumRows * Math.Abs(_affine[2]));
            }

            set
            {
                if (Width == 0 && _numColumns > 0)
                {
                    _affine[1] = value / _numColumns;
                    _affine[2] = 0;
                    return;
                }

                double columnFactor = NumColumns * Math.Abs(_affine[1]) / Width;
                double rowFactor = NumRows * Math.Abs(_affine[2]) / Width;
                double newColumnWidth = value * columnFactor;
                double newRowWidth = value * rowFactor;
                _affine[1] = Math.Sign(_affine[1]) * newColumnWidth / NumColumns;
                _affine[2] = Math.Sign(_affine[2]) * newRowWidth / NumRows;
            }
        }

        /// <inheritdoc/>
        public string WorldFile
        {
            get
            {
                return _worldFile;
            }

            set
            {
                _worldFile = Path.GetFullPath(value);
            }
        }

        /// <inheritdoc/>
        public double X
        {
            get
            {
                double xMin = double.MaxValue;
                double[] affine = AffineCoefficients; // in case this is an overridden property
                double nr = NumRows;
                double nc = NumColumns;

                // Because these coefficients can be negative, we can't make assumptions about what corner is furthest left.
                if (affine[0] < xMin) xMin = affine[0]; // TopLeft;
                if (affine[0] + (nc * affine[1]) < xMin) xMin = affine[0] + (nc * affine[1]); // TopRight;
                if (affine[0] + (nr * affine[2]) < xMin) xMin = affine[0] + (nr * affine[2]); // BottomLeft;
                if (affine[0] + (nc * affine[1]) + (nr * affine[2]) < xMin) xMin = affine[0] + (nc * affine[1]) + (nr * affine[2]); // BottomRight

                // the coordinate thus far is the center of the cell. The actual left is half a cell further left.
                return xMin - (Math.Abs(affine[1]) / 2) - (Math.Abs(affine[2]) / 2);
            }

            set
            {
                double dx = value - X;
                _affine[0] = _affine[0] + dx; // resetting affine[0] will shift everything else
            }
        }

        /// <inheritdoc/>
        public double Y
        {
            get
            {
                double yMax = double.MinValue;
                double[] affine = AffineCoefficients; // in case this is an overridden property
                double nr = NumRows;
                double nc = NumColumns;

                // Because these coefficients can be negative, we can't make assumptions about what corner is furthest left.
                if (affine[3] > yMax) yMax = affine[3]; // TopLeft;
                if (affine[3] + (nc * affine[4]) > yMax) yMax = affine[3] + (nc * affine[4]); // TopRight;
                if (affine[3] + (nr * affine[5]) > yMax) yMax = affine[3] + (nr * affine[5]); // BottomLeft;
                if (affine[3] + (nc * affine[4]) + (nr * affine[5]) > yMax) yMax = affine[3] + (nc * affine[4]) + (nr * affine[5]); // BottomRight

                // the value thus far is at the center of the cell. Return a value half a cell further
                return yMax + (Math.Abs(affine[4]) / 2) + (Math.Abs(affine[5]) / 2);
            }

            set
            {
                double dy = value - Y;
                _affine[3] += dy; // resets the dY
            }
        }

        public double Area => Width * Height;

        public double Bottom => Y - Height;

        public double Left => X;

        public double Right => X + Width;

        public double Top => Y;

        #endregion

        #region Methods

        /// <summary>
        /// Returns a duplicate of this object as an object.
        /// </summary>
        /// <returns>A duplicate of this object as an object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// Creates a duplicate of this RasterBounds class.
        /// </summary>
        /// <returns>A RasterBounds that has the same properties but does not point to the same internal array.</returns>
        public RasterBounds Copy()
        {
            var result = (RasterBounds)MemberwiseClone();
            result.AffineCoefficients = new double[6];
            for (int i = 0; i < 6; i++)
            {
                result.AffineCoefficients[i] = _affine[i];
            }

            return result;
        }

        /// <summary>
        /// Attempts to load the data from the given fileName.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        public virtual void Open(string fileName)
        {
            OpenWorldFile(fileName);
        }

        /// <inheritdoc/>
        public virtual void Save()
        {
            SaveWorldFile();
        }
        /// <summary>
        /// 扩展范围
        /// </summary>
        /// <param name="distance">距离</param>
        public void ExpandBy(double distance)
        {
            X -= distance;
            Y += distance;
            Width += distance * 2;
            Height += distance * 2;
        }
        /// <inheritdoc/>
        public void OpenWorldFile(string fileName)
        {
            WorldFile = fileName;
            double[] affine = new double[6];

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            if (line != null)
            {
                affine[1] = double.Parse(line); // Dx
            }

            line = sr.ReadLine();
            if (line != null)
            {
                affine[2] = double.Parse(line); // Skew X
            }

            line = sr.ReadLine();
            if (line != null)
            {
                affine[4] = double.Parse(line); // Skew Y
            }

            line = sr.ReadLine();
            if (line != null)
            {
                affine[5] = double.Parse(line); // Dy
            }

            line = sr.ReadLine();
            if (line != null)
            {
                affine[0] = double.Parse(line); // Top Left X
            }

            line = sr.ReadLine();
            if (line != null)
            {
                affine[3] = double.Parse(line); // Top Left Y
            }

            AffineCoefficients = affine;
            sr.Close();
        }

        /// <inheritdoc/>
        public void SaveWorldFile()
        {
            using (var sw = new StreamWriter(WorldFile))
            {
                double[] affine = AffineCoefficients;
                sw.WriteLine(affine[1]); // Dx
                sw.WriteLine(affine[2]); // rotation X
                sw.WriteLine(affine[4]); // rotation Y
                sw.WriteLine(affine[5]); // Dy
                sw.WriteLine(affine[0]); // Top Left X
                sw.WriteLine(affine[3]); // Top Left Y
            }
        }
        /// <summary>
        /// 重采样
        /// </summary>
        /// <param name="numRows">行数</param>
        /// <param name="numColumns">列数</param>
        /// <returns>栅格范围</returns>
        public IRasterBounds ResampleTransform(int numRows, int numColumns)
        {
            double[] affine = AffineCoefficients;
            double[] result = new double[6];
            double oldNumRows = NumRows;
            double oldNumColumns = NumColumns;
            result[0] = affine[0]; // Top Left X
            result[3] = affine[3]; // Top Left Y
            result[1] = affine[1] * oldNumColumns / numColumns; // dx
            result[5] = affine[5] * oldNumRows / numRows; // dy
            result[2] = affine[2] * oldNumRows / numRows; // skew x
            result[4] = affine[4] * oldNumColumns / numColumns; // skew y
            return new RasterBounds(numRows, numColumns, result);
        }
        /// <inheritdoc/>
        public void SaveAs(string fileName)
        {
            WorldFile = fileName;
            Save();
        }
        #endregion
    }
}