using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projection;
using OSGeo.GDAL;
using OSGeo.OGR;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// A GDAL raster.
    /// </summary>
    /// <typeparam name="T">Type of the contained items.</typeparam>
    [Serializable]
    public class GdalRasterSet<T> : RasterSet<T>
        where T : IEquatable<T>, IComparable<T>
    {
        private Dataset _dataset;
        /// <summary>
        /// 数据源
        /// </summary>
        public Dataset Dataset
        {
            get { return _dataset; }
            private set
            {
                if (_dataset != null)
                {
                    _dataset.Dispose();
                }
                _dataset = value;
                OnDatasetChanged();
            }
        }
        public override ProjectionInfo Projection 
        {
            get
            {
                if (base.Projection == null)
                {
                    base.Projection = new GdalProjectionInfo(_dataset.GetProjection());
                }
                return base.Projection;
            }
        }
        private void OnDatasetChanged()
        {
            int numBands = _dataset.RasterCount;
            for (int i = 1; i <= numBands; i++)
            {
                Band band = _dataset.GetRasterBand(i);
                if (i == 1)
                {
                    _band = band;
                }
                Bands.Add(new GdalRasterSet<T>(Filename, _dataset, band));
            }
            Projection = new GdalProjectionInfo(_dataset.GetProjection());
            ReadHeader();
        }
        bool _ignoreChangeDataset;
        public override string RelativeFilename
        {
            get => base.RelativeFilename;
            protected set
            {
                base.RelativeFilename = value;
                if (!_ignoreChangeDataset)
                {
                    if (File.Exists(value))
                    {
                        try
                        {
                            Dataset = Gdal.Open(value, Access.GA_Update);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                            Dataset = Gdal.Open(value, Access.GA_ReadOnly);
                        }
                    }
                    else
                    {
                        Dataset = null;
                    }
                }
            }
        }
        public override int NumRows => _dataset.RasterYSize;
        public override int NumColumns => _dataset.RasterXSize;
        #region Fields

        private Band _band;
        private int _overviewCount;
        private ColorInterp _colorInterp;
        private int _overview;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GdalRasterSet{T}"/> class.
        /// This can be a raster with multiple bands.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="fromDataset">The dataset.</param>
        public GdalRasterSet(string fileName, Dataset fromDataset)
        {
            _ignoreChangeDataset = true;
            Filename = fileName;
            _ignoreChangeDataset = false;
            Name = Path.GetFileNameWithoutExtension(fileName);
            _dataset = fromDataset;
            OnDatasetChanged();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GdalRasterSet{T}"/> class.
        /// Creates a new raster from the specified band.
        /// </summary>
        /// <param name="fileName">The string path of the file if any.</param>
        /// <param name="fromDataset">The dataset.</param>
        /// <param name="fromBand">The band.</param>
        public GdalRasterSet(string fileName, Dataset fromDataset, Band fromBand)
        {
            _dataset = fromDataset;
            _band = fromBand;
            _ignoreChangeDataset = true;
            Filename = fileName;
            _ignoreChangeDataset = false;
            Name = Path.GetFileNameWithoutExtension(fileName);
            ReadHeader();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the GDAL data type.
        /// </summary>
        public DataType GdalDataType => _band.DataType;

        /// <summary>
        /// Gets or sets the NoDataValue.
        /// </summary>
        public override double NoDataValue
        {
            get
            {
                return base.NoDataValue;
            }

            set
            {
                base.NoDataValue = value;
                if (_band != null)
                {
                    _band.SetNoDataValue(value);
                }
                else
                {
                    foreach (var raster in Bands)
                    {
                        raster.NoDataValue = value;
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This needs to return the actual image and override the base
        /// behavior that handles the internal variables only.
        /// </summary>
        /// <param name="envelope">The envelope to grab image data for.</param>
        /// <param name="window">A Rectangle</param>
        /// <returns>The image.</returns>
        public override Bitmap GetBitmap(IExtent envelope, Rectangle window)
        {
            if (window.Width == 0 || window.Height == 0)
            {
                return null;
            }

            var result = new Bitmap(window.Width, window.Height);
            using (var g = Graphics.FromImage(result))
            {
                DrawGraphics(g, envelope, window);
            }

            return result;
        }
        private void DrawGraphics(Graphics g, IExtent envelope, Rectangle window)
        {
            var layerIsDrawing = $"{Name} 绘制中...";
            ProgressHandler?.Progress(5, layerIsDrawing);

            // Gets the scaling factor for converting from geographic to pixel coordinates
            double dx = window.Width / envelope.Width;
            double dy = window.Height / envelope.Height;

            double[] a = Bounds.AffineCoefficients;

            // calculate inverse
            double p = 1 / ((a[1] * a[5]) - (a[2] * a[4]));
            double[] aInv = new double[4];
            aInv[0] = a[5] * p;
            aInv[1] = -a[2] * p;
            aInv[2] = -a[4] * p;
            aInv[3] = a[1] * p;

            // estimate rectangle coordinates
            double tlx = ((envelope.MinX - a[0]) * aInv[0]) + ((envelope.MaxY - a[3]) * aInv[1]);
            double tly = ((envelope.MinX - a[0]) * aInv[2]) + ((envelope.MaxY - a[3]) * aInv[3]);
            double trx = ((envelope.MaxX - a[0]) * aInv[0]) + ((envelope.MaxY - a[3]) * aInv[1]);
            double trY = ((envelope.MaxX - a[0]) * aInv[2]) + ((envelope.MaxY - a[3]) * aInv[3]);
            double blx = ((envelope.MinX - a[0]) * aInv[0]) + ((envelope.MinY - a[3]) * aInv[1]);
            double bly = ((envelope.MinX - a[0]) * aInv[2]) + ((envelope.MinY - a[3]) * aInv[3]);
            double brx = ((envelope.MaxX - a[0]) * aInv[0]) + ((envelope.MinY - a[3]) * aInv[1]);
            double bry = ((envelope.MaxX - a[0]) * aInv[2]) + ((envelope.MinY - a[3]) * aInv[3]);

            // get absolute maximum and minimum coordinates to make a rectangle on projected coordinates
            // that overlaps all the visible area.
            double tLx = Math.Min(Math.Min(Math.Min(tlx, trx), blx), brx);
            double tLy = Math.Min(Math.Min(Math.Min(tly, trY), bly), bry);
            double bRx = Math.Max(Math.Max(Math.Max(tlx, trx), blx), brx);
            double bRy = Math.Max(Math.Max(Math.Max(tly, trY), bly), bry);

            // limit it to the available image
            // todo: why we compare NumColumns\Rows and X,Y coordinates??
            if (tLx > Bounds.NumColumns) tLx = Bounds.NumColumns;
            if (tLy > Bounds.NumRows) tLy = Bounds.NumRows;
            if (bRx > Bounds.NumColumns) bRx = Bounds.NumColumns;
            if (bRy > Bounds.NumRows) bRy = Bounds.NumRows;

            if (tLx < 0) tLx = 0;
            if (tLy < 0) tLy = 0;
            if (bRx < 0) bRx = 0;
            if (bRy < 0) bRy = 0;

            ProgressHandler?.Progress(10, layerIsDrawing);

            // gets the affine scaling factors.
            float m11 = Convert.ToSingle(a[1] * dx);
            float m22 = Convert.ToSingle(a[5] * -dy);
            float m21 = Convert.ToSingle(a[2] * dx);
            float m12 = Convert.ToSingle(a[4] * -dy);
            double l = a[0] - (.5 * (a[1] + a[2])); // Left of top left pixel
            double t = a[3] - (.5 * (a[4] + a[5])); // top of top left pixel
            float xShift = (float)((l - envelope.MinX) * dx);
            float yShift = (float)((envelope.MaxY - t) * dy);
            g.PixelOffsetMode = PixelOffsetMode.Half;

            float xRatio = 1, yRatio = 1;
            if (_overviewCount > 0)
            {
                using (Band firstOverview = _band.GetOverview(0))
                {
                    xRatio = (float)firstOverview.XSize / _band.XSize;
                    yRatio = (float)firstOverview.YSize / _band.YSize;
                }
            }
            if (m11 > xRatio || m22 > yRatio)
            {
                // out of pyramids
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                _overview = -1; // don't use overviews when zooming behind the max res.
            }
            else
            {
                // estimate the pyramids that we need.
                // when using unreferenced images m11 or m22 can be negative resulting on inf logarithm.
                // so the Math.abs
                _overview = (int)Math.Min(Math.Log(Math.Abs(1 / m11), 2), Math.Log(Math.Abs(1 / m22), 2));

                // limit it to the available pyramids
                _overview = Math.Min(_overview, _overviewCount - 1);

                // additional test but probably not needed
                if (_overview < 0)
                {
                    _overview = -1;
                }
            }

            ProgressHandler?.Progress(15, layerIsDrawing);

            var overviewPow = Math.Pow(2, _overview + 1);
            m11 *= (float)overviewPow;
            m12 *= (float)overviewPow;
            m21 *= (float)overviewPow;
            m22 *= (float)overviewPow;
            g.Transform = new Matrix(m11, m12, m21, m22, xShift, yShift);

            int blockXsize = 0, blockYsize = 0;

            // get the optimal block size to request gdal.
            // if the image is stored line by line then ask for a 100px stripe.
            int size = 64;
            Action<Band> computeBlockSize = new Action<Band>((band) =>
            {
                band.GetBlockSize(out blockXsize, out blockYsize);
                if (blockYsize == 1)
                {
                    blockYsize = Math.Min(size, band.YSize);
                }
                //if (blockYsize < size)
                //{
                //    blockYsize = Math.Min(size, band.YSize);
                //}
                //if (blockXsize < size)
                //{
                //    blockXsize = Math.Min(size, band.XSize);
                //}
            });
            if (_overview >= 0 && _overviewCount > 0)
            {
                using (var overview = _band.GetOverview(_overview))
                {
                    computeBlockSize(overview);
                }
            }
            else
            {
                computeBlockSize(_band);
            }

            int nbX, nbY;

            // witdh and height of the image
            var w = (bRx - tLx) / overviewPow;
            var h = (bRy - tLy) / overviewPow;

            // limit the block size to the viewable image.
            if (w < blockXsize)
            {
                blockXsize = (int)Math.Ceiling(w);
                nbX = 1;
            }
            else if (w == blockXsize)
            {
                nbX = 1;
            }
            else
            {
                nbX = (int)Math.Ceiling(w / blockXsize);
            }

            if (h < blockYsize)
            {
                blockYsize = (int)Math.Ceiling(h);
                nbY = 1;
            }
            else if (h == blockYsize)
            {
                nbY = 1;
            }
            else
            {
                nbY = (int)Math.Ceiling(h / blockYsize);
            }
            int redundancy = (int)Math.Ceiling(1 / Math.Min(m11, m22));

            ProgressHandler?.Progress(20, $"{Name} 绘制中...");
            var increment = 70.0 / nbX / nbY;
            double progressPercent = 20;

            for (var i = 0; i < nbX; i++)
            {
                for (var j = 0; j < nbY; j++)
                {
                    // The +1 is to remove the white stripes artifacts
                    double xOffsetD = (tLx / overviewPow) + (i * blockXsize);
                    double yOffsetD = (tLy / overviewPow) + (j * blockYsize);
                    int xOffsetI = (int)Math.Floor(xOffsetD);
                    int yOffsetI = (int)Math.Floor(yOffsetD);
                    int xSize = blockXsize + redundancy;
                    int ySize = blockYsize + redundancy;
                    using (var bitmap = GetBitmap(xOffsetI, yOffsetI, xSize, ySize))
                    {
                        if (bitmap != null)
                        {
                            g.DrawImage(bitmap, xOffsetI, yOffsetI);
                        }
                    }
                    progressPercent += increment;
                    ProgressHandler?.Progress((int)progressPercent, $"{Name} 绘制中...");
                }
            }
            ProgressHandler?.Progress(99, $"{Name} 绘制中...");
        }
        private unsafe Bitmap GetBitmap(int width, int height, byte[] rBuffer, byte[] gBuffer, byte[] bBuffer, byte[] aBuffer = null)
        {
            if (width <= 0 || height <= 0)
            {
                return null;
            }
            Bitmap result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bData = result.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            byte* scan0 = (byte*)bData.Scan0;
            int stride = bData.Stride;
            int dWidth = stride - width * 4;
            int ptrIndex = -1;
            int bufferIndex = -1;
            if (aBuffer == null)
            {
                for (int row = 0; row < height; row++)
                {
                    ptrIndex = row * stride;
                    bufferIndex = row * width;
                    for (int col = 0; col < width; col++)
                    {
                        byte bValue = bBuffer[bufferIndex];
                        byte gValue = gBuffer[bufferIndex];
                        byte rValue = rBuffer[bufferIndex];
                        byte aValue = 255;
                        if (rValue == NoDataValue)
                        {
                            aValue = 0;
                        }
                        scan0[ptrIndex] = bValue;
                        scan0[ptrIndex + 1] = gValue;
                        scan0[ptrIndex + 2] = rValue;
                        scan0[ptrIndex + 3] = aValue;
                        ptrIndex += 4;
                        bufferIndex++;
                    }
                }
            }
            else
            {
                for (int row = 0; row < height; row++)
                {
                    ptrIndex = row * stride;
                    bufferIndex = row * width;
                    for (int col = 0; col < width; col++)
                    {
                        byte bValue = bBuffer[bufferIndex];
                        byte gValue = gBuffer[bufferIndex];
                        byte rValue = rBuffer[bufferIndex];
                        byte aValue = aBuffer[bufferIndex];
                        if (rValue == NoDataValue)
                        {
                            aValue = 0;
                        }
                        scan0[ptrIndex] = bValue;
                        scan0[ptrIndex + 1] = gValue;
                        scan0[ptrIndex + 2] = rValue;
                        scan0[ptrIndex + 3] = aValue;
                        ptrIndex += 4;
                        bufferIndex++;
                    }
                }
            }
            result.UnlockBits(bData);
            return result;
        }
        private Bitmap ReadGrayIndex(int xOffset, int yOffset, int xSize, int ySize)
        {
            Band firstBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                firstBand = _band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                firstBand = _band;
            }
            int width, height;
            GdalExtentions.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, firstBand, out width, out height);
            byte[] rBuffer = GdalExtentions.ReadBand(firstBand, xOffset, yOffset, width, height);
            if (disposeBand)
            {
                firstBand.Dispose();
            }
            Bitmap result = GetBitmap(width, height, rBuffer, rBuffer, rBuffer);
            return result;
        }
        private Bitmap ReadRgb(int xOffset, int yOffset, int xSize, int ySize)
        {
            if (Bands.Count < 3)
            {
                throw new Exception("RGB Format was indicated but there are only " + Bands.Count + " bands!");
            }
            Band rBand;
            Band gBand;
            Band bBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                rBand = (Bands[0] as GdalRasterSet<T>)._band.GetOverview(_overview);
                gBand = (Bands[1] as GdalRasterSet<T>)._band.GetOverview(_overview);
                bBand = (Bands[2] as GdalRasterSet<T>)._band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                rBand = (Bands[0] as GdalRasterSet<T>)._band;
                gBand = (Bands[1] as GdalRasterSet<T>)._band;
                bBand = (Bands[2] as GdalRasterSet<T>)._band;
            }

            int width, height;
            GdalExtentions.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, rBand, out width, out height);
            byte[] rBuffer = GdalExtentions.ReadBand(rBand, xOffset, yOffset,  width, height);
            byte[] gBuffer = GdalExtentions.ReadBand(gBand, xOffset, yOffset,  width, height);
            byte[] bBuffer = GdalExtentions.ReadBand(bBand, xOffset, yOffset, width, height);
            if (disposeBand)
            {
                rBand.Dispose();
                gBand.Dispose();
                bBand.Dispose();
            }
            Bitmap result = GetBitmap(width, height, rBuffer, gBuffer, bBuffer);
            return result;
        }

        private Bitmap ReadArgb(int xOffset, int yOffset, int xSize, int ySize)
        {
            if (Bands.Count < 4)
            {
                throw new Exception("ARGB Format was indicated but there are only " + Bands.Count + " bands!");
            }
            Band aBand;
            Band rBand;
            Band gBand;
            Band bBand;
            var disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                aBand = (Bands[0] as GdalRasterSet<T>)._band.GetOverview(_overview);
                rBand = (Bands[1] as GdalRasterSet<T>)._band.GetOverview(_overview);
                gBand = (Bands[2] as GdalRasterSet<T>)._band.GetOverview(_overview);
                bBand = (Bands[3] as GdalRasterSet<T>)._band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                aBand = (Bands[0] as GdalRasterSet<T>)._band;
                rBand = (Bands[1] as GdalRasterSet<T>)._band;
                gBand = (Bands[2] as GdalRasterSet<T>)._band;
                bBand = (Bands[3] as GdalRasterSet<T>)._band;
            }

            int width, height;
            GdalExtentions.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, rBand, out width, out height);
            byte[] aBuffer = GdalExtentions.ReadBand(aBand, xOffset, yOffset, width, height);
            byte[] rBuffer = GdalExtentions.ReadBand(rBand, xOffset, yOffset, width, height);
            byte[] gBuffer = GdalExtentions.ReadBand(gBand, xOffset, yOffset, width, height);
            byte[] bBuffer = GdalExtentions.ReadBand(bBand, xOffset, yOffset,  width, height);
            if (disposeBand)
            {
                aBand.Dispose();
                rBand.Dispose();
                gBand.Dispose();
                bBand.Dispose();
            }
            Bitmap result = GetBitmap(width, height, rBuffer, gBuffer, bBuffer, aBuffer);
            return result;
        }
        private Bitmap ReadPaletteBuffered(int xOffset, int yOffset, int xSize, int ySize)
        {
            ColorTable ct = _band.GetRasterColorTable();
            if (ct == null)
            {
                throw new Exception("Image was stored with a palette interpretation but has no color table.");
            }

            if (ct.GetPaletteInterpretation() != PaletteInterp.GPI_RGB)
            {
                throw new Exception("Only RGB palette interpretation is currently supported by this " + " plug-in, " + ct.GetPaletteInterpretation() + " is not supported.");
            }

            int count = ct.GetCount();
            byte[][] colorTable = new byte[ct.GetCount()][];
            for (int i = 0; i < count; i++)
            {
                using (ColorEntry ce = ct.GetColorEntry(i))
                {
                    colorTable[i] = new[] { (byte)ce.c4, (byte)ce.c1, (byte)ce.c2, (byte)ce.c3 };
                }
            }
            ct.Dispose();

            Band firstBand;
            bool disposeBand = false;
            if (_overview >= 0 && _overviewCount > 0)
            {
                firstBand = _band.GetOverview(_overview);
                disposeBand = true;
            }
            else
            {
                firstBand = _band;
            }

            int width, height;
            GdalExtentions.NormalizeSizeToBand(xOffset, yOffset, xSize, ySize, firstBand, out width, out height);
            byte[] indexBuffer = GdalExtentions.ReadBand(firstBand, xOffset, yOffset, width, height);
            if (disposeBand)
            {
                firstBand.Dispose();
            }
            byte[] rBuffer = new byte[indexBuffer.Length];
            byte[] gBuffer = new byte[indexBuffer.Length];
            byte[] bBuffer = new byte[indexBuffer.Length];
            byte[] aBuffer = new byte[indexBuffer.Length];
            for (int i = 0; i < indexBuffer.Length; i++)
            {
                int index = indexBuffer[i];
                aBuffer[i] = colorTable[index][0];
                rBuffer[i] = colorTable[index][1];
                gBuffer[i] = colorTable[index][2];
                bBuffer[i] = colorTable[index][3];
            }
            Bitmap result = GetBitmap(width, height, rBuffer, gBuffer, gBuffer, aBuffer);
            return result;
        }
        /// <summary>
        /// Gets a block of data directly, converted into a bitmap.  This always writes
        /// to the base layer, not the overviews.
        /// </summary>
        /// <param name="xOffset">The zero based integer column offset from the left</param>
        /// <param name="yOffset">The zero based integer row offset from the top</param>
        /// <param name="xSize">The integer number of pixel columns in the block. </param>
        /// <param name="ySize">The integer number of pixel rows in the block.</param>
        /// <returns>A Bitmap that is xSize, ySize.</returns>
        private Bitmap GetBitmap(int xOffset, int yOffset, int xSize, int ySize)
        {
            Bitmap result = null;
            Action action = new Action(() =>
            {
                switch (BandCount)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                        result = ReadGrayIndex(xOffset, yOffset, xSize, ySize);
                        break;
                    case 3:
                        result = ReadRgb(xOffset, yOffset, xSize, ySize);
                        break;
                    default:
                        result = ReadArgb(xOffset, yOffset, xSize, ySize);
                        break;
                }
            });
            switch (_colorInterp)
            {
                case ColorInterp.GCI_PaletteIndex:
                    result = ReadPaletteBuffered(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_GrayIndex:
                    result = ReadGrayIndex(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_RedBand:
                    result = ReadRgb(xOffset, yOffset, xSize, ySize);
                    break;
                case ColorInterp.GCI_AlphaBand:
                    result = ReadArgb(xOffset, yOffset, xSize, ySize);
                    break;
                default:
                    action.Invoke();
                    break;
            }
            // data set disposed on disposing this image
            return result;
        }
        /// <summary>
        /// Gets the category colors.
        /// </summary>
        /// <returns>The category colors.</returns>
        public override Color[] CategoryColors()
        {
            Color[] colors = null;
            ColorTable table = GetColorTable();
            if (table != null)
            {
                int colorCount = table.GetCount();
                if (colorCount > 0)
                {
                    colors = new Color[colorCount];
                    for (int colorIndex = 0; colorIndex < colorCount; colorIndex += 1)
                    {
                        colors[colorIndex] = Color.DimGray;
                        ColorEntry entry = table.GetColorEntry(colorIndex);
                        switch (table.GetPaletteInterpretation())
                        {
                            case PaletteInterp.GPI_RGB:
                                colors[colorIndex] = Color.FromArgb(entry.c4, entry.c1, entry.c2, entry.c3);
                                break;
                            case PaletteInterp.GPI_Gray:
                                colors[colorIndex] = Color.FromArgb(255, entry.c1, entry.c1, entry.c1);
                                break;

                                // TODO: do any files use these types?
                                // case PaletteInterp.GPI_HLS
                                // case PaletteInterp.GPI_CMYK
                        }
                    }
                }
            }

            return colors;
        }

        /// <summary>
        /// Gets the category names.
        /// </summary>
        /// <returns>The category names.</returns>
        public override string[] CategoryNames()
        {
            if (_band != null)
            {
                return _band.GetCategoryNames();
            }

            foreach (GdalRasterSet<T> raster in Bands)
            {
                return raster._band.GetCategoryNames();
            }

            return null;
        }
       
        /// <summary>
        /// Gets the mean, standard deviation, minimum and maximum
        /// </summary>
        public override Statistics GetStatistics()
        {
            Statistics statistics = new Statistics();
            if (_band != null)
            {
                double min, max, mean, std;
                CPLErr err;
                try
                {
                    err = _band.GetStatistics(0, 1, out min, out max, out mean, out std);
                    if (err != CPLErr.CE_None)
                    {
                        err = _band.ComputeStatistics(false, out min, out max, out mean, out std, null, null);
                    }
                    statistics.Minimum = min;
                    statistics.Maximum = max;
                    statistics.Mean = mean;
                    statistics.StdDeviation = std;
                }
                catch (Exception ex)
                {
                    err = CPLErr.CE_Failure;
                    max = min = std = mean = 0;
                    Trace.WriteLine(ex);
                }

                // http://dotspatial.codeplex.com/workitem/22221
                // GetStatistics didn't return anything, so try use the raster default method.
                if (err != CPLErr.CE_None || (max == 0 && min == 0 && std == 0 && mean == 0)) base.GetStatistics();
            }
            else
            {
                // ?? doesn't this mean the stats get overwritten several times.
                foreach (IRasterSet raster in Bands)
                {
                    statistics = raster.GetStatistics();
                }
            }
            return statistics;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)。
                    if (_band != null)
                    {
                        _band.Dispose();
                    }
                    else
                    {
                        foreach (IRasterSet raster in Bands)
                        {
                            raster.Dispose();
                        }
                    }

                    if (_dataset != null)
                    {
                        _dataset.FlushCache();
                        _dataset.Dispose();
                    }
                }
                // 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // 将大型字段设置为 null。
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the callback progress content.
        /// </summary>
        /// <param name="complete">Percent of completeness.</param>
        /// <param name="message">Message is not used.</param>
        /// <param name="data">Data is not used.</param>
        /// <returns>0</returns>
        private int GdalProgressFunc(double complete, IntPtr message, IntPtr data)
        {
            ProgressHandler?.Progress(Convert.ToInt32(complete), "Copy Progress");
            return 0;
        }

        private ColorTable GetColorTable()
        {
            if (_band != null)
            {
                return _band.GetColorTable();
            }

            foreach (GdalRasterSet<T> raster in Bands)
            {
                return raster._band.GetColorTable();
            }

            return null;
        }

        private void ReadHeader()
        {
            // Todo: look for prj file if GetProjection returns null.
            // Do we need to read this as an Esri string if we don't get a proj4 string?
            if (_band != null)
            {
                RasterType = _band.DataType.ToRasterType();
                   double val;
                _band.GetNoDataValue(out val, out int hasVal);
                if (hasVal == 1)
                {
                    NoDataValue = val;
                }
                _overviewCount = _band.GetOverviewCount();
                _colorInterp = _band.GetColorInterpretation();
                int maxPixels = 2048 * 2048;
                if (_overviewCount <= 0 && NumColumns * NumRows > maxPixels)
                {
                    int ret = _dataset.CreateOverview();
                    _overviewCount = _band.GetOverviewCount();
                }
            }

            double[] affine = new double[6];
            _dataset.GetGeoTransform(affine);

            // in gdal (row,col) coordinates are defined relative to the top-left corner of the top-left cell
            // shift them by half a cell to give coordinates relative to the center of the top-left cell
            affine = new AffineTransform(affine).TransfromToCorner(0.5, 0.5);
            Bounds = new RasterBounds(NumRows, NumColumns, affine);
            PixelSpace = Marshal.SizeOf(typeof(T));
        }

        #endregion
    }
}