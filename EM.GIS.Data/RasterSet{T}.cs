using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EM.GIS.Data
{
    /// <summary>
    /// 泛型栅格数据集
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    [Serializable]
    public class RasterSet<T> : RasterSet
        where T : IEquatable<T>, IComparable<T>
    {
        #region Constructors

        /// <summary>
        /// 初始化
        /// </summary>
        public RasterSet()
        {
            NoDataValue = Global.ToDouble(Global.MinimumValue<T>());
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public override int ByteSize => GetByteSize<T>();

        #endregion

        #region Methods

        /// <summary>
        /// 读取栅格
        /// </summary>
        /// <param name="xOff"></param>
        /// <param name="yOff"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual T[][] ReadRaster(int xOff, int yOff, int sizeX, int sizeY)
        {
            throw new NotImplementedException("This should be overridden by classes that specify a file format.");
        }

        /// <summary>
        /// 写入栅格
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="xOff"></param>
        /// <param name="yOff"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void WriteRaster(T[][] buffer, int xOff, int yOff, int xSize, int ySize)
        {
            throw new NotImplementedException("This should be overridden by classes that specify a file format.");
        }


        #endregion
    }
}