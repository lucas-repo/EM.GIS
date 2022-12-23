using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace EM.GIS.Data
{
    /// <summary>
    /// 栅格数据集
    /// </summary>
    [Serializable]
    public abstract class RasterSet : DataSet, IRasterSet
    {
        /// <inheritdoc/>
        public virtual int NumRows { get; }
        /// <inheritdoc/>
        public virtual int NumColumns { get; }
        /// <inheritdoc/>
        public IList<IRasterSet> Bands { get; }
        /// <summary>
        /// 字节大小
        /// </summary>
        public abstract int ByteSize { get; }
        /// <inheritdoc/>
        [Category("Data")]
        [Description("Gets or sets a  double showing the no-data value for this raster.")]
        public virtual double? NoDataValue { get; set; }
        /// <inheritdoc/>
        public IRasterBounds Bounds { get; set; }
        /// <inheritdoc/>
        public override IExtent Extent
        {
            get => Bounds.Extent;
            set => Bounds.Extent = value;
        }
        /// <summary>
        /// 像素间隔
        /// </summary>
        public int PixelSpace { get; set; }
        /// <summary>
        /// 每行间隔
        /// </summary>
        public int LineSpace { get; set; }
        /// <inheritdoc/>
        public RasterType RasterType { get; set; }

        /// <inheritdoc/>
        public int BandCount => Bands.Count;

        public RasterSet()
        {
            Bands = new List<IRasterSet>();
        }

        /// <summary>
        /// 获取分类颜色
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Color[] CategoryColors()
        {
            return null;
        }
        /// <summary>
        /// 获取分类集合
        /// </summary>
        /// <returns></returns>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string[] CategoryNames()
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual Statistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取字节大小
        /// </summary>
        /// <typeparam name="TValue">类型</typeparam>
        /// <param name="value">值</param>
        /// <returns>字节大小</returns>
        public static int GetByteSize<TValue>(TValue value)
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
        /// <inheritdoc/>
        public virtual void Draw(Graphics graphics, RectangleF rectangle, IExtent extent, Action<int> progressAction = null, Func<bool> cancelFunc = null)
        { }
    }
}
