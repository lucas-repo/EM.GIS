using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EM.GIS.Geometries
{
    //
    // 摘要:
    //     /// A lightweight class used to store coordinates on the 2-dimensional Cartesian
    //     plane. ///
    //     /// It is distinct from GeoAPI.Geometries.IPoint, which is a subclass of GeoAPI.Geometries.IGeometry.
    //     /// Unlike objects of type GeoAPI.Geometries.IPoint (which contain additional
    //     /// information such as an envelope, a precision model, and spatial reference
    //     /// system information), a Coordinate only contains ordinate values /// and propertied.
    //     ///
    //     ///
    //     /// Coordinates are two-dimensional points, with an additional Z-ordinate. ///
    //     If an Z-ordinate value is not specified or not defined, /// constructed coordinates
    //     have a Z-ordinate of NaN /// (which is also the value of GeoAPI.Geometries.Coordinate.NullOrdinate).
    //     ///
    //     ///
    //
    // 言论：
    //     /// Apart from the basic accessor functions, NTS supports /// only specific operations
    //     involving the Z-ordinate. ///
    [Serializable]
    public class Coordinate : ICoordinate, ICloneable, IComparable, IComparable<Coordinate>, IEquatable<Coordinate>
    {
        //
        // 摘要:
        //     /// The value used to indicate a null or missing ordinate value. /// In particular,
        //     used for the value of ordinates for dimensions /// greater than the defined dimension
        //     of a coordinate. ///
        public const double NullOrdinate = double.NaN;

        //
        // 摘要:
        //     /// The X or horizontal, or longitudinal ordinate ///
        public double X { get; set; }

        //
        // 摘要:
        //     /// The Y or vertical, or latitudinal ordinate ///
        public double Y { get; set; }

        //
        // 摘要:
        //     /// The Z or up or altitude ordinate ///
        public double Z { get; set; }

        //
        // 摘要:
        //     /// An optional place holder for a measure value if needed ///
        public double M { get; set; }

        //
        // 摘要:
        //     /// This indicates that the coordinate can have max 4 Ordinates (X,Y,Z,M). ///
        public int MaxPossibleOrdinates => 4;

        public double this[int index]
        {
            get
            {
                double value = double.NaN;
                if (index >= 0 && index < MaxPossibleOrdinates)
                {
                    switch (index)
                    {
                        case 0:
                            value = X;
                            break;
                        case 1:
                            value = Y;
                            break;
                        case 2:
                            value = Z;
                            break;
                        case 3:
                            value = M;
                            break;
                    }
                }
                return value;
            }
            set
            {
                if (index >= 0 && index < MaxPossibleOrdinates)
                {
                    switch (index)
                    {
                        case 0:
                            X = value;
                            break;
                        case 1:
                            Y = value;
                            break;
                        case 2:
                            Z = value;
                            break;
                        case 3:
                            M = value;
                            break;
                    }
                }
            }
        }
        //
        // 摘要:
        //     /// Gets/Sets Coordinates (x,y,z) values. ///
        public Coordinate CoordinateValue
        {
            get
            {
                return this;
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                M = value.M;
            }
        }

        //
        // 摘要:
        //     /// This is not a true length, but simply tests the Z and M value. If the M value
        //     is not NaN this is 4. Else if Z value /// is NaN then the value is 2. Otherwise
        //     this is 3. ///
        public int NumOrdinates
        {
            get
            {
                if (!double.IsNaN(M))
                {
                    return 4;
                }
                if (!double.IsNaN(Z))
                {
                    return 3;
                }
                return 2;
            }
        }



        //
        // 摘要:
        //     /// Constructs a Coordinate at (x,y,z). ///
        //
        // 参数:
        //   x:
        //     X value.
        //
        //   y:
        //     Y value.
        //
        //   z:
        //     Z value.
        public Coordinate(double x, double y, double z)
            : this(x, y, z, double.NaN)
        {
        }

        //
        // 摘要:
        //     /// Creates a new instance of Coordinate ///
        public Coordinate(double x, double y, double z, double m)
        {
            X = x;
            Y = y;
            Z = z;
            M = m;
        }

        //
        // 摘要:
        //     /// Creates an coordinate with (0,0,NaN,NaN). ///
        public Coordinate()
            : this(0.0, 0.0, double.NaN, double.NaN)
        {
        }


        // 摘要:
        //     /// Constructs a Coordinate having the same (x,y,z) values as /// other. ///
        //
        // 参数:
        //   c:
        //     Coordinate to copy.
        public Coordinate(Coordinate c)
            : this(c.X, c.Y, c.Z, c.M)
        {
        }

        //
        // 摘要:
        //     /// Creates a 2D Coordinate with NaN for the Z and M values. ///
        //
        // 参数:
        //   x:
        //     X value.
        //
        //   y:
        //     Y value.
        public Coordinate(double x, double y)
            : this(x, y, double.NaN, double.NaN)
        {
        }

        //
        // 摘要:
        //     /// Returns whether the planar projections of the two Coordinates are equal.
        //     ///
        //
        // 参数:
        //   other:
        //     Coordinate with which to do the 2D comparison.
        //
        // 返回结果:
        //     /// true if the x- and y-coordinates are equal; /// the Z coordinates do not
        //     have to be equal. ///
        public bool Equals2D(Coordinate other)
        {
            if (other == null)
            {
                return false;
            }
            if (X == other.X)
            {
                return Y == other.Y;
            }
            return false;
        }

        //
        // 摘要:
        //     /// Tests if another coordinate has the same value for X and Y, within a tolerance.
        //     ///
        //
        // 参数:
        //   c:
        //     A GeoAPI.Geometries.Coordinate.
        //
        //   tolerance:
        //     The tolerance value.
        //
        // 返回结果:
        //     true if the X and Y ordinates are within the given tolerance.
        //
        // 言论：
        //     The Z ordinate is ignored.
        public bool Equals2D(Coordinate c, double tolerance)
        {
            if (c == null)
            {
                return false;
            }
            if (!EqualsWithTolerance(X, c.X, tolerance))
            {
                return false;
            }
            if (!EqualsWithTolerance(Y, c.Y, tolerance))
            {
                return false;
            }
            return true;
        }

        //
        // 摘要:
        //     /// Checks whether the difference between x1 and x2 is smaller than tolerance.
        //     ///
        //
        // 参数:
        //   x1:
        //     First value used for check.
        //
        //   x2:
        //     Second value used for check.
        //
        //   tolerance:
        //     The difference must me smaller this value, for the x-values to be considered
        //     equal.
        private static bool EqualsWithTolerance(double x1, double x2, double tolerance)
        {
            return Math.Abs(x1 - x2) <= tolerance;
        }

        //
        // 摘要:
        //     /// Returns true if other has the same values for the x and y ordinates. ///
        //     Since Coordinates are 2.5D, this routine ignores the z value when making the
        //     comparison. ///
        //
        // 参数:
        //   other:
        //     Coordinate with which to do the comparison.
        //
        // 返回结果:
        //     true if other is a Coordinate with the same values for the x and y ordinates.
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            Coordinate coordinate = other as Coordinate;
            if (coordinate != null)
            {
                return Equals(coordinate);
            }
            if (!(other is Coordinate))
            {
                return false;
            }
            return ((IEquatable<Coordinate>)this).Equals((Coordinate)other);
        }

        //
        // 摘要:
        //     /// Returns true if other has the same values for the x and y ordinates. ///
        //     Since Coordinates are 2.5D, this routine ignores the z value when making the
        //     comparison. ///
        //
        // 参数:
        //   other:
        //     Coordinate with which to do the comparison.
        //
        // 返回结果:
        //     true if other is a Coordinate with the same values for the x and y ordinates.
        public bool Equals(Coordinate other)
        {
            return Equals2D(other);
        }

        //
        // 摘要:
        //     /// Compares this object with the specified object for order. /// Since Coordinates
        //     are 2.5D, this routine ignores the z value when making the comparison. /// Returns
        //     /// -1 : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan
        //     other.y)) /// 0 : this.x == other.x AND this.y = other.y /// 1 : this.x greaterthan
        //     other.x || ((this.x == other.x) AND (this.y greaterthan other.y)) ///
        //
        // 参数:
        //   o:
        //     Coordinate with which this Coordinate is being compared.
        //
        // 返回结果:
        //     /// A negative integer, zero, or a positive integer as this Coordinate /// is
        //     less than, equal to, or greater than the specified Coordinate. ///
        public int CompareTo(object o)
        {
            Coordinate other = (Coordinate)o;
            return CompareTo(other);
        }

        //
        // 摘要:
        //     /// Compares this object with the specified object for order. /// Since Coordinates
        //     are 2.5D, this routine ignores the z value when making the comparison. /// Returns
        //     /// -1 : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan
        //     other.y)) /// 0 : this.x == other.x AND this.y = other.y /// 1 : this.x greaterthan
        //     other.x || ((this.x == other.x) AND (this.y greaterthan other.y)) ///
        //
        // 参数:
        //   other:
        //     Coordinate with which this Coordinate is being compared.
        //
        // 返回结果:
        //     /// A negative integer, zero, or a positive integer as this Coordinate /// is
        //     less than, equal to, or greater than the specified Coordinate. ///
        public int CompareTo(Coordinate other)
        {
            if (other == null)
            {
                return 1;
            }
            if (X < other.X)
            {
                return -1;
            }
            if (X > other.X)
            {
                return 1;
            }
            if (Y < other.Y)
            {
                return -1;
            }
            if (!(Y > other.Y))
            {
                return 0;
            }
            return 1;
        }

        //
        // 摘要:
        //     /// Returns true if other /// has the same values for X, Y and Z. ///
        //
        // 参数:
        //   other:
        //     A GeoAPI.Geometries.Coordinate with which to do the 3D comparison.
        //
        // 返回结果:
        //     /// true if other is a GeoAPI.Geometries.Coordinate /// with the same values
        //     for X, Y and Z. ///
        public bool Equals3D(Coordinate other)
        {
            if (other == null)
            {
                return false;
            }
            if (X == other.X && Y == other.Y)
            {
                if (Z != other.Z)
                {
                    if (double.IsNaN(Z))
                    {
                        return double.IsNaN(other.Z);
                    }
                    return false;
                }
                return true;
            }
            return false;
        }

        //
        // 摘要:
        //     /// Tests if another coordinate has the same value for Z, within a tolerance.
        //     ///
        //
        // 参数:
        //   c:
        //     A GeoAPI.Geometries.Coordinate.
        //
        //   tolerance:
        //     The tolerance value.
        //
        // 返回结果:
        //     true if the Z ordinates are within the given tolerance.
        public bool EqualInZ(Coordinate c, double tolerance)
        {
            return EqualsWithTolerance(Z, c.Z, tolerance);
        }

        //
        // 摘要:
        //     /// Returns a string of the form (x,y,z) . ///
        //
        // 返回结果:
        //     string of the form (x,y,z)
        public override string ToString()
        {
            return "(" + X.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + Y.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + Z.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + M.ToString("R", NumberFormatInfo.InvariantInfo) + ")";
        }

        //
        // 摘要:
        //     /// Create a new object as copy of this instance. ///
        public object Clone()
        {
            return MemberwiseClone();
        }

        //
        // 摘要:
        //     /// Computes the 2-dimensional Euclidean distance to another location. ///
        //
        // 参数:
        //   c:
        //     A GeoAPI.Geometries.Coordinate with which to do the distance comparison.
        //
        // 返回结果:
        //     the 2-dimensional Euclidean distance between the locations.
        //
        // 言论：
        //     The Z-ordinate is ignored.
        public double Distance(Coordinate c)
        {
            double num = X - c.X;
            double num2 = Y - c.Y;
            return Math.Sqrt(num * num + num2 * num2);
        }

        //
        // 摘要:
        //     /// Computes the 3-dimensional Euclidean distance to another location. ///
        //
        // 参数:
        //   c:
        //     A GeoAPI.Geometries.Coordinate with which to do the distance comparison.
        //
        // 返回结果:
        //     the 3-dimensional Euclidean distance between the locations.
        public double Distance3D(Coordinate c)
        {
            double num = X - c.X;
            double num2 = Y - c.Y;
            double num3 = Z - c.Z;
            return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        //
        // 摘要:
        //     /// Gets a hashcode for this coordinate. ///
        //
        // 返回结果:
        //     A hashcode for this coordinate.
        public override int GetHashCode()
        {
            int num = 17;
            num = 37 * num + GetHashCode(X);
            num = 37 * num + GetHashCode(Y);
            if (!double.IsNaN(Z))
            {
                num = 37 * num + GetHashCode(Z);
            }
            return num;
        }

        //
        // 摘要:
        //     /// Computes a hash code for a double value, using the algorithm from /// Joshua
        //     Bloch's book Effective Java". ///
        //
        // 参数:
        //   value:
        //     A hashcode for the double value
        public static int GetHashCode(double value)
        {
            long num = BitConverter.DoubleToInt64Bits(value);
            return (int)(num ^ (num >> 32));
        }

        //
        // 摘要:
        //     /// If either X or Y is defined as NaN, then this coordinate is considered empty.
        //     ///
        public bool IsEmpty()
        {
            if (!double.IsNaN(X))
            {
                return double.IsNaN(Y);
            }
            return true;
        }
        public double[] ToDoubleArray(int dimension = 2)
        {
            double[] array = null;
            if (dimension >= 0&&dimension<MaxPossibleOrdinates)
            {
                array = new double[dimension];
                for (int i = 0; i < MaxPossibleOrdinates; i++)
                {
                    array[i] = this[i];
                }
            }
            return array;
        }

        //
        // 摘要:
        //     /// Compares this object with the specified object for order. /// Since Coordinates
        //     are 2.5D, this routine ignores the z value when making the comparison. /// Returns
        //     /// -1 : this.x lowerthan other.x || ((this.x == other.x) AND (this.y lowerthan
        //     other.y)) /// 0 : this.x == other.x AND this.y = other.y /// 1 : this.x greaterthan
        //     other.x || ((this.x == other.x) AND (this.y greaterthan other.y)) ///
        //
        // 参数:
        //   o:
        //     Coordinate with which this Coordinate is being compared.
        //
        // 返回结果:
        //     /// A negative integer, zero, or a positive integer as this Coordinate /// is
        //     less than, equal to, or greater than the specified Coordinate. ///
        int IComparable.CompareTo(object o)
        {
            Coordinate other = (Coordinate)o;
            return CompareTo(other);
        }


    }
}

