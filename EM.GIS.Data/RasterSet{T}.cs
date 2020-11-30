using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EM.GIS.Data
{
    /// <summary>
    /// A raster of the given type.
    /// </summary>
    /// <typeparam name="T">Type of the raster.</typeparam>
    [Serializable]
    public class RasterSet<T> : RasterSet
        where T : IEquatable<T>, IComparable<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasterSet{T}"/> class.
        /// </summary>
        public RasterSet()
        {
            NoDataValue = Global.ToDouble(Global.MinimumValue<T>());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the size of each raster element in bytes.
        /// </summary>
        /// <remarks>
        /// This only works for a few numeric types, and will return 0 if it is not identifiable as one
        /// of these basic types: byte, short, int, long, float, double, decimal, sbyte, ushort, uint, ulong, bool.
        /// </remarks>
        public override int ByteSize => GetByteSize(default(T));

        #endregion

        #region Methods

        /// <summary>
        /// This Method should be overrridden by classes, and provides the primary ability.
        /// </summary>
        /// <param name="xOff">The horizontal offset of the area to read values from.</param>
        /// <param name="yOff">The vertical offset of the window to read values from.</param>
        /// <param name="sizeX">The number of values to read into the buffer.</param>
        /// <param name="sizeY">The vertical size of the window to read into the buffer.</param>
        /// <returns>The jagged array of raster values of type T.</returns>
        public virtual T[][] ReadRaster(int xOff, int yOff, int sizeX, int sizeY)
        {
            throw new NotImplementedException("This should be overridden by classes that specify a file format.");
        }

        /// <summary>
        /// This method reads the values from the entire band into an array and returns the array as a single array.
        /// This specifies a window where the xSize and ySize specified and 0 is used for the pixel and line space.
        /// </summary>
        /// <param name="buffer">The one dimensional array of values containing all the data for this particular content.</param>
        /// <param name="xOff">The horizontal offset of the area to read values from.</param>
        /// <param name="yOff">The vertical offset of the window to read values from.</param>
        /// <param name="xSize">The number of values to read into the buffer.</param>
        /// <param name="ySize">The vertical size of the window to read into the buffer.</param>
        public virtual void WriteRaster(T[][] buffer, int xOff, int yOff, int xSize, int ySize)
        {
            throw new NotImplementedException("This should be overridden by classes that specify a file format.");
        }

        private static int GetByteSize(object value)
        {
            if (value is byte) return 1;
            if (value is short) return 2;
            if (value is int) return 4;
            if (value is long) return 8;
            if (value is float) return 4;
            if (value is double) return 8;

            if (value is sbyte) return 1;
            if (value is ushort) return 2;
            if (value is uint) return 4;
            if (value is ulong) return 8;

            if (value is bool) return 1;

            return 0;
        }

        #endregion
    }
}