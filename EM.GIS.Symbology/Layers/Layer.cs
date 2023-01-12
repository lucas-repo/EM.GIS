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
    public abstract class Layer : LegendItem, ILayer
    {
        public ICategory DefaultCategory
        {
            get
            {
                ICategory category = null;
                if (Children?.Count > 0)
                {
                    category = Children[Children.Count - 1];
                }
                return category;
            }
            set
            {
                if (Children != null)
                {
                    if (Children.Count > 0)
                    {
                        Children[Children.Count - 1] = value;
                    }
                    else
                    {
                        Children.Add(value);
                    }
                }
            }
        }
        public virtual ISelection Selection { get; protected set; }
        public new IGroup Parent
        {
            get => base.Parent as IGroup;
            set => base.Parent = value;
        }
        public new ICategoryCollection Children
        {
            get => base.Children as ICategoryCollection;
            protected set => base.Children = value;
        }

        public virtual IExtent Extent { get; set; }

        public bool UseDynamicVisibility { get; set; }
        public double MaxInverseScale { get; set; }
        public double MinInverseScale { get; set; }
        private IDataSet _dataSet;

        public IDataSet DataSet
        {
            get { return _dataSet; }
            set
            {
                if (SetProperty(ref _dataSet, value))
                {
                    OnDataSetChanged();
                }
            }
        }

        private string _progressMessage;
        /// <summary>
        /// 进度消息文字
        /// </summary>
        protected string ProgressMessage
        {
            get
            {
                if (string.IsNullOrEmpty(_progressMessage))
                {
                    _progressMessage = $"绘制 {Text} 中...";
                }
                return _progressMessage;
            }
        }
        private void OnDataSetChanged()
        {
            IExtent extent = null;
            if (DataSet != null)
            {
                extent = DataSet.Extent;
            }
            Extent = extent;
        }

        public Layer()
        {
        }
        public Layer(IDataSet dataSet) : this()
        {
            DataSet = dataSet;
        }
        /// <inheritdoc/>
        public void Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (mapArgs == null || mapArgs.Graphics == null ||  mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() ||!GetVisible(mapArgs.DestExtent)|| cancelFunc?.Invoke() == true)
            {
                return;
            }
            progressAction?.Invoke(ProgressMessage, 0);
            MapArgs destMapArgs = mapArgs.Copy();
            if (mapArgs.Projection != null && DataSet?.Projection != null && !mapArgs.Projection.Equals(DataSet.Projection))
            {
                destMapArgs.Extent = mapArgs.DestExtent.Copy();
                mapArgs.Projection.ReProject(DataSet.Projection, destMapArgs.Extent);
                destMapArgs.DestExtent = mapArgs.DestExtent.Copy();
                mapArgs.Projection.ReProject(DataSet.Projection, destMapArgs.DestExtent);
            }
            OnDraw(destMapArgs, selected, progressAction, cancelFunc, invalidateMapFrameAction);
            progressAction?.Invoke(ProgressMessage, 100);
        }
        /// <summary>
        /// 绘制图层到画布
        /// </summary>
        /// <param name="mapArgs">参数</param>
        /// <param name="selected">是否绘制选择</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消匿名方法</param>
        /// <param name="invalidateMapFrameAction">使地图无效匿名方法</param>
        protected abstract void OnDraw(MapArgs mapArgs, bool selected = false, Action<string, int> progressAction = null, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null);
        /// <inheritdoc/>
        public bool GetVisible(IExtent extent, Rectangle rectangle)
        {
            bool visible = false;
            if (!IsVisible)
            {
                return visible;
            }

            if (UseDynamicVisibility)
            {
                //todo compare the scalefactor 
            }

            return true;
        }
        /// <summary>
        /// 判断在指定范围是否可见
        /// </summary>
        /// <param name="extent">范围</param>
        /// <returns>是否可见</returns>
        public bool GetVisible(IExtent extent)
        {
            bool ret = GetVisible();
            if (!ret)
            {
                return ret;
            }
            ret = Extent.Intersects(extent);
            return ret;
        }

        #region IDisposable Support
        /// <summary>
        /// 是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }
        /// <inheritdoc/>
        public IFrame Frame { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    if (_dataSet != null)
                    {
                        _dataSet.Dispose();
                        _dataSet = null;
                    }
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                IsDisposed = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~BaseLayer()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}