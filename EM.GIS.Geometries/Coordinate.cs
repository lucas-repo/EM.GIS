using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 坐标点
    /// </summary>
    [Serializable]
    public class Coordinate : ICoordinate
    {
        /// <inheritdoc/>
        public double X { get; set; }

        /// <inheritdoc/>
        public double Y { get; set; }

        /// <inheritdoc/>
        public double Z { get; set; } = double.NaN;

        /// <inheritdoc/>
        public double M { get; set; } = double.NaN;

        /// <inheritdoc/>
        public int MaxPossibleOrdinates => 4;

        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public int Dimension
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

        public Coordinate()
        {
        }
        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }
        public Coordinate(double x, double y, double z) : this(x, y)
        {
            Z = z;
        }

        public Coordinate(double x, double y, double z, double m) : this(x, y, z)
        {
            M = m;
        }
        public Coordinate(Coordinate c) : this(c.X, c.Y, c.Z, c.M)
        {
        }
        public Coordinate(double[] array)
        {
            if (array != null)
            {
                int count = Math.Min(array.Length, MaxPossibleOrdinates);
                for (int i = 0; i < count; i++)
                {
                    this[i] = array[i];
                }
            }
        }

        /// <inheritdoc/>
        public bool Equals2D(ICoordinate other)
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

        /// <inheritdoc/>
        public bool Equals2D(ICoordinate c, double tolerance)
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
        /// <summary>
        /// 判断是否近似相等
        /// </summary>
        /// <param name="x1">x1</param>
        /// <param name="x2">x2</param>
        /// <param name="tolerance">近似值</param>
        /// <returns>近似相等为true反之false</returns>
        private static bool EqualsWithTolerance(double x1, double x2, double tolerance)
        {
            return Math.Abs(x1 - x2) <= tolerance;
        }
        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }
            var coordinate = other as ICoordinate;
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

        /// <inheritdoc/>
        public bool Equals(ICoordinate other)
        {
            return Equals2D(other);
        }

        /// <inheritdoc/>
        public int CompareTo(object o)
        {
            var other = o as ICoordinate;
            return CompareTo(other);
        }
        /// <inheritdoc/>
        public int CompareTo(ICoordinate other)
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

        /// <inheritdoc/>
        public bool Equals3D(ICoordinate c, double tolerance)
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
            if (!EqualsWithTolerance(Z, c.Z, tolerance))
            {
                return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "(" + X.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + Y.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + Z.ToString("R", NumberFormatInfo.InvariantInfo) + ", " + M.ToString("R", NumberFormatInfo.InvariantInfo) + ")";
        }

        /// <inheritdoc/>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <inheritdoc/>
        public double Distance(ICoordinate c)
        {
            double num = X - c.X;
            double num2 = Y - c.Y;
            return Math.Sqrt(num * num + num2 * num2);
        }

        /// <inheritdoc/>
        public double Distance3D(ICoordinate c)
        {
            double num = X - c.X;
            double num2 = Y - c.Y;
            double num3 = Z - c.Z;
            return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public static int GetHashCode(double value)
        {
            long num = BitConverter.DoubleToInt64Bits(value);
            return (int)(num ^ (num >> 32));
        }

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            if (!double.IsNaN(X))
            {
                return double.IsNaN(Y);
            }
            return true;
        }
        /// <inheritdoc/>
        public double[] ToDoubleArray(int dimension = 2)
        {
            double[] array = null;
            if (dimension >= 0 && dimension < MaxPossibleOrdinates)
            {
                array = new double[dimension];
                for (int i = 0; i < MaxPossibleOrdinates; i++)
                {
                    array[i] = this[i];
                }
            }
            return array;
        }

        /// <inheritdoc/>
        int IComparable.CompareTo(object o)
        {
            Coordinate other = (Coordinate)o;
            return CompareTo(other);
        }
    }
}

