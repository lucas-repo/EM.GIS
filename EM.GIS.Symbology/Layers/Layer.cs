using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;


namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层
    /// </summary>
    [Serializable]
    public abstract class Layer : LegendItem, ILayer
    {
        public virtual IExtent Extent { get; set; }

        public ICategory DefaultCategory
        {
            get
            {
                ICategory category = null;
                if (Categories?.Count > 0)
                {
                    category = Categories[Categories.Count - 1];
                }
                return category;
            }
            set
            {
                if (Categories != null)
                {
                    if (Categories.Count > 0)
                    {
                        Categories[Categories.Count - 1] = value;
                    }
                    else
                    {
                        Categories.Add(value);
                    }
                }
            }
        }
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


        public ICategoryCollection Categories { get => LegendItems as ICategoryCollection; }
        public virtual ISelection Selection { get; protected set; }
        public Layer()
        { }
        public Layer(IDataSet dataSet)
        {
            DataSet = dataSet;
        }
        private void GetResolution(IExtent envelope, int pixelWidth, int pixelHeight, out double xRes, out double yRes)
        {
            double worldWidth = envelope.MaxX - envelope.MinX;
            double worldHeight = envelope.MaxY - envelope.MinY;
            xRes = worldWidth / pixelWidth;
            yRes = worldHeight / pixelHeight;
        }

        public void Draw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            if (graphics == null || rectangle.Width * rectangle.Height == 0 || extent == null || extent.Width * extent.Height == 0 || cancelFunc?.Invoke() == true)
            {
                return;
            }
            OnDraw(graphics, rectangle, extent, selected, cancelFunc,  invalidateMapFrameAction );
        }
        protected abstract void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null);
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

        #region IDisposable Support
        /// <summary>
        /// 是否已释放
        /// </summary>
        public bool IsDisposed { get; private set; }

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