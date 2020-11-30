using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 范围
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Extent : IExtent
    {
        public Extent()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Extent"/> class from the specified ordinates.
        /// </summary>
        /// <param name="xMin">The minimum X value.</param>
        /// <param name="yMin">The minimum Y value.</param>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        public Extent(double xMin, double yMin, double xMax, double yMax)
        {
            MinX = xMin;
            MinY = yMin;
            MaxX = xMax;
            MaxY = yMax;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Extent"/> class based on the given values.
        /// </summary>
        /// <param name="values">Values used to initialize XMin, YMin, XMax, YMax in the given order.</param>
        /// <param name="offset">Offset indicates at which position we can find MinX. The other values follow directly after that.</param>
        public Extent(double[] values, int offset)
        {
            if (values.Length < 4 + offset) throw new IndexOutOfRangeException("The length of the array of double values should be greater than or equal to 4 plus the value of the offset.");

            MinX = values[0 + offset];
            MinY = values[1 + offset];
            MaxX = values[2 + offset];
            MaxY = values[3 + offset];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Extent"/> class.
        /// </summary>
        /// <param name="values">Values used to initialize XMin, YMin, XMax, YMax in the given order.</param>
        public Extent(double[] values)
        {
            if (values.Length < 4) throw new IndexOutOfRangeException("The length of the array of double values should be greater than or equal to 4.");

            MinX = values[0];
            MinY = values[1];
            MaxX = values[2];
            MaxY = values[3];
        }


        #region  Properties

        /// <summary>
        /// Gets the Center of this extent.
        /// </summary>
        public ICoordinate Center
        {
            get
            {
                double x = MinX + ((MaxX - MinX) / 2);
                double y = MinY + ((MaxY - MinY) / 2);
                return new Coordinate(x, y);
            }
            set
            {
                SetCenter(value);
            }
        }

        public bool HasM
        {
            get
            {
                if (double.IsNaN(MinM) || double.IsNaN(MaxM))
                {
                    return false;
                }

                return MinM <= MaxM;
            }
        }

        public bool HasZ
        {
            get
            {
                if (double.IsNaN(MinZ) || double.IsNaN(MaxZ))
                {
                    return false;
                }

                return MinZ <= MaxZ;
            }
        }

        /// <summary>
        /// Gets or sets the height. Getting this returns MaxY - MinY. Setting this will update MinY, keeping MaxY the same. (Pinned at top left corner).
        /// </summary>
        public double Height
        {
            get
            {
                return MaxY - MinY;
            }

            set
            {
                MinY = MaxY - value;
            }
        }

        public double MaxX { get; set; } = double.NaN;
        public double MaxY { get; set; } = double.NaN;
        public double MaxM { get; set; } = double.NaN;
        public double MaxZ { get; set; } = double.NaN;

        public double MinX { get; set; } = double.NaN;
        public double MinY { get; set; } = double.NaN;
        public double MinM { get; set; } = double.NaN;
        public double MinZ { get; set; } = double.NaN;

        public double Width
        {
            get
            {
                return MaxX - MinX;
            }

            set
            {
                MaxX = MinX + value;
            }
        }

        /// <summary>
        /// Gets or sets the X. Getting this returns MinX. Setting this will shift both MinX and MaxX, keeping the width the same.
        /// </summary>
        public double X
        {
            get
            {
                return MinX;
            }

            set
            {
                double w = Width;
                MinX = value;
                Width = w;
            }
        }

        /// <summary>
        /// Gets or sets the Y. Getting this will return MaxY. Setting this will shift both MinY and MaxY, keeping the height the same.
        /// </summary>
        public double Y
        {
            get
            {
                return MaxY;
            }

            set
            {
                double h = Height;
                MaxY = value;
                Height = h;
            }
        }

        #endregion

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="left">First extent to test.</param>
        /// <param name="right">Second extent to test.</param>
        /// <returns>True, if the extents equal.</returns>
        public static bool operator ==(Extent left, IExtent right)
        {
            if ((object)left == null) return right == null;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality test
        /// </summary>
        /// <param name="left">First extent to test.</param>
        /// <param name="right">Second extent to test.</param>
        /// <returns>True, if the extents do not equal.</returns>
        public static bool operator !=(Extent left, IExtent right)
        {
            if ((object)left == null) return right != null;
            return !left.Equals(right);
        }

        #region Methods

        /// <summary>
        /// This allows parsing the X and Y values from a string version of the extent as: 'X[-180|180], Y[-90|90]'
        /// Where minimum always precedes maximum. The correct M or MZ version of extent will be returned if the string has those values.
        /// </summary>
        /// <param name="text">The string text to parse.</param>
        /// <returns>The parsed extent.</returns>
        /// <exception cref="ExtentParseException">Is thrown if the string could not be parsed to an extent.</exception>
        public static IExtent Parse(string text)
        {
            IExtent result;
            if (TryParse(text, out result, out _)) return result;

            throw new Exception("读取失败");
        }

        /// <summary>
        /// This allows parsing the X and Y values from a string version of the extent as: 'X[-180|180], Y[-90|90]'
        /// Where minimum always precedes maximum. The correct M or MZ version of extent will be returned if the string has those values.
        /// </summary>
        /// <param name="text">Text that contains the extent values.</param>
        /// <param name="result">Extent that was created.</param>
        /// <param name="nameFailed">Indicates which value failed.</param>
        /// <returns>True if the string could be parsed to an extent.</returns>
        public static bool TryParse(string text, out IExtent result, out string nameFailed)
        {
            double xmin, xmax, ymin, ymax, mmin, mmax;
            result = new Extent();
            if (text.Contains("Z"))
            {
                double zmin, zmax;
                nameFailed = "Z";
                if (!TryExtract(text, "Z", out zmin, out zmax)) return false;
                result.MinZ = zmin;
                result.MaxZ = zmax;
                nameFailed = "M";
                if (!TryExtract(text, "M", out mmin, out mmax)) return false;
                result.MinM = mmin;
                result.MaxM = mmax;
            }
            if (text.Contains("M"))
            {
                nameFailed = "M";
                if (!TryExtract(text, "M", out mmin, out mmax)) return false;
                result.MinM = mmin;
                result.MaxM = mmax;
            }
            else
            {
                result = new Extent();
            }
            nameFailed = "X";
            if (!TryExtract(text, "X", out xmin, out xmax)) return false;
            result.MinX = xmin;
            result.MaxX = xmax;
            nameFailed = "Y";
            if (!TryExtract(text, "Y", out ymin, out ymax)) return false;
            result.MinY = ymin;
            result.MaxY = ymax;
            return true;
        }

        /// <summary>
        /// Produces a clone, rather than using this same object.
        /// </summary>
        /// <returns>The cloned Extent.</returns>
        public virtual object Clone()
        {
            return new Extent(MinX, MinY, MaxX, MaxY);
        }

        /// <summary>
        /// Tests if the specified extent is contained by this extent.
        /// </summary>
        /// <param name="ext">Extent that might be contained.</param>
        /// <returns>True if this extent contains the specified extent.</returns>
        /// <exception cref="ArgumentNullException">Thrown if ext is null.</exception>
        public virtual bool Contains(IExtent ext)
        {
            if (Equals(ext, null)) throw new ArgumentNullException(nameof(ext));

            return Contains(ext.MinX, ext.MaxX, ext.MinY, ext.MaxY);
        }

        /// <summary>
        /// Tests if the specified coordinate is contained by this extent.
        /// </summary>
        /// <param name="c">The coordinate to test.</param>
        /// <returns>True if this extent contains the specified coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if c is null.</exception>
        public virtual bool Contains(ICoordinate c)
        {
            if (Equals(c, null)) throw new ArgumentNullException(nameof(c));
            return Contains(c.X, c.X, c.Y, c.Y);
        }


        /// <summary>
        /// Copies the MinX, MaxX, MinY, MaxY values from extent.
        /// </summary>
        /// <param name="extent">Any IExtent implementation.</param>
        public virtual void CopyFrom(IExtent extent)
        {
            if (Equals(extent, null)) throw new ArgumentNullException(nameof(extent));

            MinX = extent.MinX;
            MaxX = extent.MaxX;
            MinY = extent.MinY;
            MaxY = extent.MaxY;
        }

        /// <summary>
        /// Checks whether this extent and the specified extent are equal.
        /// </summary>
        /// <param name="obj">Second Extent to check.</param>
        /// <returns>True, if extents are the same (either both null or equal in all X and Y values).</returns>
        public override bool Equals(object obj)
        {
            // Check the identity case for reference equality
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            if (base.Equals(obj)) return true;

            IExtent other = obj as IExtent;
            if (other == null) return false;

            return MinX == other.MinX && MinY == other.MinY && MaxX == other.MaxX && MaxY == other.MaxY;
        }

        /// <summary>
        /// Expand will adjust both the minimum and maximum by the specified sizeX and sizeY
        /// </summary>
        /// <param name="padX">The amount to expand left and right.</param>
        /// <param name="padY">The amount to expand up and down.</param>
        public void ExpandBy(double padX, double padY)
        {
            MinX -= padX;
            MaxX += padX;
            MinY -= padY;
            MaxY += padY;
        }

        /// <summary>
        /// This expand the extent by the specified padding on all bounds. So the width will
        /// change by twice the padding for instance. To Expand only x and y, use
        /// the overload with those values explicitly specified.
        /// </summary>
        /// <param name="padding">The double padding to expand the extent.</param>
        public virtual void ExpandBy(double padding)
        {
            MinX -= padding;
            MaxX += padding;
            MinY -= padding;
            MaxY += padding;
        }

        /// <summary>
        /// Expands this extent to include the domain of the specified extent.
        /// </summary>
        /// <param name="ext">The extent to include.</param>
        public virtual void ExpandToInclude(IExtent ext)
        {
            if (ext == null) return;

            ExpandToInclude(ext.MinX, ext.MaxX, ext.MinY, ext.MaxY);
        }

        /// <summary>
        /// Expands this extent to include the domain of the specified point.
        /// </summary>
        /// <param name="x">The x value to include.</param>
        /// <param name="y">The y value to include.</param>
        public void ExpandToInclude(double x, double y)
        {
            ExpandToInclude(x, x, y, y);
        }

        /// <summary>
        /// Spreads the values for the basic X, Y extents across the whole range of int.
        /// Repetition will occur, but it should be rare.
        /// </summary>
        /// <returns>Integer</returns>
        public override int GetHashCode()
        {
            // 215^4 ~ Int.MaxValue so the value will cover the range based mostly on first 2 sig figs.
            int xmin = Convert.ToInt32((MinX * 430 / MinX) - 215);
            int xmax = Convert.ToInt32((MaxX * 430 / MaxX) - 215);
            int ymin = Convert.ToInt32((MinY * 430 / MinY) - 215);
            int ymax = Convert.ToInt32((MaxY * 430 / MaxY) - 215);
            return xmin * xmax * ymin * ymax;
        }

        /// <summary>
        /// Calculates the intersection of this extent and the other extent. A result
        /// with a min greater than the max in either direction is considered invalid
        /// and represents no intersection.
        /// </summary>
        /// <param name="other">The other extent to intersect with.</param>
        /// <returns>The resulting extent.</returns>
        public virtual IExtent Intersection(IExtent other)
        {
            if (Equals(other, null)) throw new ArgumentNullException(nameof(other));

            Extent result = new Extent
            {
                MinX = MinX > other.MinX ? MinX : other.MinX,
                MaxX = MaxX < other.MaxX ? MaxX : other.MaxX,
                MinY = MinY > other.MinY ? MinY : other.MinY,
                MaxY = MaxY < other.MaxY ? MaxY : other.MaxY
            };
            return result;
        }

        /// <summary>
        /// Tests if this extent intersects the specified coordinate.
        /// </summary>
        /// <param name="c">The coordinate that might intersect this extent.</param>
        /// <returns>True if this extent intersects the specified coordinate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if c is null.</exception>
        public virtual bool Intersects(ICoordinate c)
        {
            if (Equals(c, null)) throw new ArgumentNullException(nameof(c));

            return Intersects(c.X, c.Y);
        }

        /// <summary>
        /// Tests for intersection with the specified coordinate.
        /// </summary>
        /// <param name="x">The double ordinate to test intersection with in the X direction.</param>
        /// <param name="y">The double ordinate to test intersection with in the Y direction.</param>
        /// <returns>True if a point with the specified x and y coordinates is within or on the border
        /// of this extent object. NAN values will always return false.</returns>
        public bool Intersects(double x, double y)
        {
            if (double.IsNaN(x) || double.IsNaN(y)) return false;

            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
        }

        /// <summary>
        /// Tests if this extent intersects the specified extent.
        /// </summary>
        /// <param name="ext">The extent that might intersect this extent.</param>
        /// <returns>True if this extent intersects the specified extent.</returns>
        /// <exception cref="ArgumentNullException">Thrown if ext is null.</exception>
        public virtual bool Intersects(IExtent ext)
        {
            if (Equals(ext, null)) throw new ArgumentNullException(nameof(ext));

            return Intersects(ext.MinX, ext.MaxX, ext.MinY, ext.MaxY);
        }

        /// <summary>
        /// If this is undefined, it will have a min that is larger than the max, or else any value is NaN.
        /// This only applies to the X and Y terms. Check HasM or HasZ for higher dimensions.
        /// </summary>
        /// <returns>Boolean, true if the envelope has not had values set for it yet.</returns>
        public bool IsEmpty()
        {
            if (double.IsNaN(MinX) || double.IsNaN(MaxX) || double.IsNaN(MinY) || double.IsNaN(MaxY)) return true;
            return MinX > MaxX || MinY > MaxY; // Simplified 
        }

        /// <summary>
        /// This centers the X and Y aspect of the extent on the specified center location.
        /// </summary>
        /// <param name="centerX">The X value of the center coordinate to set.</param>
        /// <param name="centerY">The Y value of the center coordinate to set.</param>
        /// <param name="width">The new extent width.</param>
        /// <param name="height">The new extent height.</param>
        public void SetCenter(double centerX, double centerY, double width, double height)
        {
            MinX = centerX - (width / 2);
            MaxX = centerX + (width / 2);
            MinY = centerY - (height / 2);
            MaxY = centerY + (height / 2);
        }

        /// <summary>
        /// This centers the X and Y aspect of the extent on the specified center location.
        /// </summary>
        /// <param name="center">The center coordinate to set.</param>
        /// <param name="width">The new extent width.</param>
        /// <param name="height">The new extent height.</param>
        /// <exception cref="ArgumentNullException">Thrown if center is null.</exception>
        public void SetCenter(ICoordinate center, double width, double height)
        {
            if (Equals(center, null)) throw new ArgumentNullException(nameof(center));

            SetCenter(center.X, center.Y, width, height);
        }

        /// <summary>
        /// This centers the extent on the specified coordinate, keeping the width and height the same.
        /// </summary>
        /// <param name="center">Center value which is used to center the extent.</param>
        /// <exception cref="ArgumentNullException">Thrown if center is null.</exception>
        public void SetCenter(ICoordinate center)
        {
            // prevents NullReferenceException when accessing center.X and center.Y
            if (Equals(center, null)) throw new ArgumentNullException(nameof(center));

            SetCenter(center.X, center.Y, Width, Height);
        }


        /// <summary>
        /// Creates a string that shows the extent.
        /// </summary>
        /// <returns>The string form of the extent.</returns>
        public override string ToString()
        {
            return "X[" + MinX + "|" + MaxX + "], Y[" + MinY + "|" + MaxY + "]";
        }

        /// <summary>
        /// Tests if this extent is within the specified extent.
        /// </summary>
        /// <param name="ext">Extent that might contain this extent.</param>
        /// <returns>True if this extent is within the specified extent.</returns>
        /// <exception cref="ArgumentNullException">Thrown if ext is null.</exception>
        public virtual bool Within(IExtent ext)
        {
            if (Equals(ext, null)) throw new ArgumentNullException(nameof(ext));

            return Within(ext.MinX, ext.MaxX, ext.MinY, ext.MaxY);
        }

       
        /// <summary>
        /// Attempts to extract the min and max from one element of text. The element should be
        /// formatted like X[1.5|2.7] using an invariant culture.
        /// </summary>
        /// <param name="entireText">Complete text from which the values should be parsed.</param>
        /// <param name="name">The name of the dimension, like X.</param>
        /// <param name="min">The minimum that gets assigned</param>
        /// <param name="max">The maximum that gets assigned</param>
        /// <returns>Boolean, true if the parse was successful.</returns>
        private static bool TryExtract(string entireText, string name, out double min, out double max)
        {
            int i = entireText.IndexOf(name, StringComparison.Ordinal);
            i += name.Length + 1;
            int j = entireText.IndexOf(']', i);
            string vals = entireText.Substring(i, j - i);
            return TryParseExtremes(vals, out min, out max);
        }

        /// <summary>
        /// Attempts to extract the min and max from the text. The text should be formatted like 1.5|2.7 using an invariant culture.
        /// </summary>
        /// <param name="numeric">Text that should be parsed.</param>
        /// <param name="min">The minimum that gets assigned.</param>
        /// <param name="max">The maximum that gets assigned.</param>
        /// <returns>True, if the numeric was parsed successfully.</returns>
        private static bool TryParseExtremes(string numeric, out double min, out double max)
        {
            string[] res = numeric.Split('|');
            max = double.NaN;
            if (!double.TryParse(res[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out min)) return false;
            if (res.Length < 2) return false;
            if (!double.TryParse(res[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out max)) return false;
            return true;
        }

        /// <summary>
        /// Tests if the specified extent is contained by this extent.
        /// </summary>
        /// <param name="minX">MinX value of the extent that might be contained.</param>
        /// <param name="maxX">MaxX value of the extent that might be contained.</param>
        /// <param name="minY">MinY value of the extent that might be contained.</param>
        /// <param name="maxY">MaxY value of the extent that might be contained.</param>
        /// <returns>True if this extent contains the specified extent.</returns>
        public bool Contains(double minX, double maxX, double minY, double maxY)
        {
            return minX >= MinX && maxX <= MaxX && minY >= MinY && maxY <= MaxY;
        }

        /// <summary>
        /// Expands this extent to include the domain of the specified extent.
        /// </summary>
        /// <param name="minX">MinX value of the extent that might intersect this extent.</param>
        /// <param name="maxX">MaxX value of the extent that might intersect this extent.</param>
        /// <param name="minY">MinY value of the extent that might intersect this extent.</param>
        /// <param name="maxY">MaxY value of the extent that might intersect this extent.</param>
        private void ExpandToInclude(double minX, double maxX, double minY, double maxY)
        {
            if (double.IsNaN(MinX) || minX < MinX) MinX = minX;
            if (double.IsNaN(MinY) || minY < MinY) MinY = minY;
            if (double.IsNaN(MaxX) || maxX > MaxX) MaxX = maxX;
            if (double.IsNaN(MaxY) || maxY > MaxY) MaxY = maxY;
        }

        /// <summary>
        /// Tests if this extent intersects the specified extent.
        /// </summary>
        /// <param name="minX">MinX value of the extent that might intersect this extent.</param>
        /// <param name="maxX">MaxX value of the extent that might intersect this extent.</param>
        /// <param name="minY">MinY value of the extent that might intersect this extent.</param>
        /// <param name="maxY">MaxY value of the extent that might intersect this extent.</param>
        /// <returns>True if this extent intersects the specified extent.</returns>
        public bool Intersects(double minX, double maxX, double minY, double maxY)
        {
            return maxX >= MinX && minX <= MaxX && maxY >= MinY && minY <= MaxY;
        }

        /// <summary>
        /// Tests if this extent is within the specified extent.
        /// </summary>
        /// <param name="minX">MinX value of the extent that might contain this extent.</param>
        /// <param name="maxX">MaxX value of the extent that might contain this extent.</param>
        /// <param name="minY">MinY value of the extent that might contain this extent.</param>
        /// <param name="maxY">MaxY value of the extent that might contain this extent.</param>
        /// <returns>True if this extent is within the specified extent.</returns>
        public bool Within(double minX, double maxX, double minY, double maxY)
        {
            return MinX >= minX && MaxX <= maxX && MinY >= minY && MaxY <= maxY;
        }

        #endregion
    }
}
