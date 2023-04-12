using BruTile;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace EM.GIS.Data
{
    /// <summary>
    /// 栅格数据集
    /// </summary>
    [Serializable]
    public abstract class RasterSet : DataSet, IRasterSet
    {
        /// <inheritdoc/>
        public virtual int Height { get; }
        /// <inheritdoc/>
        public virtual int Width { get; }
        private IEnumerable<IRasterSet>? rasterSets;
        /// <inheritdoc/>
        public virtual IEnumerable<IRasterSet> Rasters 
        {
            get
            {
                if (rasterSets == null)
                {
                    throw new Exception($"{nameof(Rasters)}不能为空");
                }
                else
                {
                    return rasterSets;
                }
            }
        }
        /// <summary>
        /// 字节大小
        /// </summary>
        public virtual int ByteSize => GetByteSize<byte>();
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
        public int BandCount => Rasters.Count();

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
        /// <inheritdoc/>
        public virtual void SetGeoTransform(double[] affine)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取字节大小
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>字节大小</returns>
        public static int GetByteSize<T>()
        {
            int ret = Marshal.SizeOf(typeof(T)) / 8;
            return ret;
            //Type type = typeof(T);
            //if (type.Equals(typeof(byte))) return 1;
            //if (value is short) return 2;
            //if (value is int) return 4;
            //if (value is long) return 8;
            //if (value is float) return 4;
            //if (value is double) return 8;

            //if (value is sbyte) return 1;
            //if (value is ushort) return 2;
            //if (value is uint) return 4;
            //if (value is ulong) return 8;

            //if (value is bool) return 1;

            //return 0;
        }
        /// <inheritdoc/>
        public Rectangle Draw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? graphicsUpdatedAction = null, Dictionary<string, object>? options = null)
        {
            var ret = Rectangle.Empty;
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || !Extent.Intersects(mapArgs.DestExtent) || cancelFunc?.Invoke() == true)
            {
                return ret;
            }
            ret = OnDraw(mapArgs,  progressAction, cancelFunc, graphicsUpdatedAction,options);
            return ret;
        }
        /// <summary>
        /// 根据指定的范围，将当前内容绘制到指定的画布
        /// </summary>
        /// <param name="mapArgs">参数</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <param name="graphicsUpdatedAction">画布更新后的匿名方法</param>
        /// <param name="options">可选参数</param>
        /// <returns>绘制的区域</returns>
        protected virtual Rectangle OnDraw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? graphicsUpdatedAction = null, Dictionary<string, object>? options = null)
        {
            return Rectangle.Empty;
        }
        /// <inheritdoc/>
        public virtual void WriteRaster(string filename, RasterArgs readArgs, RasterArgs writeArgs)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public virtual void BuildOverviews(int minWidth = 2560, int minHeight = 2560)
        { }
    }
}
