using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IFrame))]
    public class Frame : Group, IFrame
    {
        bool firstLayerAdded;
        /// <inheritdoc/>
        public IView View { get; }
        private IProjection projection;
        /// <inheritdoc/>
        public IProjection Projection
        {
            get { return projection; }
            set { SetProperty(ref projection, value); }
        }
        /// <summary>
        /// 投影工厂
        /// </summary>
        private IProjectionFactory ProjectionFactory { get; }

        /// <inheritdoc/>
        public event EventHandler? FirstLayerAdded;

        private bool _isDirty;
        /// <inheritdoc/>
        public bool IsDirty
        {
            get { return _isDirty; }
            protected set
            {
                _isDirty = value;
            }
        }
        /// <inheritdoc/>
        public string FileName { get; } = string.Empty;
        /// <summary>
        /// 实例化<seealso cref="Frame"/>
        /// </summary>
        /// <param name="projectionFactory">投影工厂</param>
        public Frame(IProjectionFactory projectionFactory)
        {
            Children.CollectionChanged += Layers_CollectionChanged;
            ProjectionFactory = projectionFactory;
            Text = "地图框";
            View = new View(this);
            Frame = this;
            projection = projectionFactory.GetProjection(4326);//默认4326
        }

        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (!firstLayerAdded)
                    {
                        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                        {
                            foreach (var item in e.NewItems)
                            {
                                if (item is ILayer)
                                {
                                    firstLayerAdded = true;
                                    break;
                                }
                            }
                        }
                        if (firstLayerAdded)
                        {
                            #region 设置投影
                            if (e.NewItems.Count > 0 && e.NewItems[0] is ILayer layer&& layer.DataSet!=null)
                            {
                                Projection = layer.DataSet.Projection.Copy();
                            }
                            #endregion
                            FirstLayerAdded?.Invoke(this, EventArgs.Empty);
                            return;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public IExtent GetMaxExtent(bool expand = false)
        {
            // to prevent exception when zoom to map with one layer with one point
            if (Extent.IsEmpty())
            {
                return new Extent();
            }
            const double Eps = 1e-7;
            var maxExtent = Extent.Width < Eps || Extent.Height < Eps ? new Extent(Extent.MinX - Eps, Extent.MinY - Eps, Extent.MaxX + Eps, Extent.MaxY + Eps) : Extent.Copy();
            if (expand) maxExtent.ExpandBy(maxExtent.Width / 10, maxExtent.Height / 10);
            return maxExtent;
        }

        /// <inheritdoc/>
        public void ExpandExtent(IExtent extent)
        {
            if (!extent.IsEmpty())
            {
                extent.ExpandBy(extent.Width / 10, extent.Height / 10);
            }
        }

        /// <inheritdoc/>
        public void New()
        {
            Children.Clear();
            firstLayerAdded = false;
            IsDirty = false;
        }

        /// <inheritdoc/>
        public void Open(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Save()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        //public bool ExtentsInitialized { get; set; }

        //public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.AntiAlias;
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    //释放托管资源
                    View.Dispose();
                    Children.CollectionChanged -= Layers_CollectionChanged;
                }
            }
            base.Dispose(disposing);
        }
    }
}
