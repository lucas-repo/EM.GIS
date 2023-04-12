using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层
    /// </summary>
    [Serializable]
    public abstract class Layer : RenderableItem, ILayer
    {
        /// <inheritdoc/>
        public new ICategoryCollection Children
        {
            get
            {
                if (base.Children is ICategoryCollection categories)
                {
                    return categories;
                }
                else
                {
                    throw new Exception($"{nameof(Children)}类型必须为{nameof(ICategoryCollection)}");
                }
            }
            protected set => base.Children = value;
        }

        private IDataSet? _dataSet;
        /// <inheritdoc/>
        public IDataSet? DataSet
        {
            get { return _dataSet; }
            set { SetProperty(ref _dataSet, value); }
        }

        /// <inheritdoc/>
        public override IExtent Extent
        {
            get
            {
                IExtent? destExtent = null;
                if (DataSet != null)
                {
                    destExtent = DataSet.Extent.Copy();
                    if (Frame?.Projection != null && DataSet.Projection != null && !Equals(Frame.Projection, DataSet.Projection))
                    {
                        DataSet.Projection.ReProject(Frame.Projection, destExtent);
                    }
                }
                if (destExtent != null)
                {
                    return destExtent;
                }
                else
                {
                    return new Extent();
                }
            }
        }

        /// <summary>
        /// 实例化<seealso cref="Layer"/>
        /// </summary>
        public Layer()
        {
        }
        /// <summary>
        /// 实例化<seealso cref="Layer"/>
        /// </summary>
        /// <param name="dataSet">数据集</param>
        public Layer(IDataSet dataSet)
        {
            _dataSet = dataSet ?? throw new ArgumentNullException(nameof(dataSet));
            Text = dataSet.Name;
        }
        /// <inheritdoc/>
        public override Rectangle Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null)
        {
            var ret = Rectangle.Empty;
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || !GetVisible(mapArgs.DestExtent) || cancelFunc?.Invoke() == true)
            {
                return ret;
            }
            progressAction?.Invoke(ProgressMessage, 0);
            MapArgs destMapArgs = mapArgs;
            var mapProjection = Frame?.Projection;
            if (mapProjection != null && DataSet?.Projection != null && !mapProjection.Equals(DataSet.Projection))
            {
                destMapArgs = mapArgs.Copy();
                //destMapArgs.Extent = mapArgs.DestExtent.Copy();
                //mapProjection.ReProject(DataSet.Projection, destMapArgs.Extent);
                destMapArgs.DestExtent = mapArgs.DestExtent.Copy();
                mapProjection.ReProject(DataSet.Projection, destMapArgs.DestExtent);
            }
            ret = OnDraw(destMapArgs, selected, progressAction, cancelFunc, invalidateMapFrameAction);
            progressAction?.Invoke(ProgressMessage, 100);
            return ret;
        }
        /// <summary>
        /// 绘制图层到画布
        /// </summary>
        /// <param name="mapArgs">基于当前图层的绘制参数</param>
        /// <param name="selected">是否绘制选择</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消匿名方法</param>
        /// <param name="invalidateMapFrameAction">使地图无效匿名方法</param>
        /// <returns>返回绘制的区域，未绘制则返回空矩形</returns>
        protected abstract Rectangle OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null);
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                DataSet?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}