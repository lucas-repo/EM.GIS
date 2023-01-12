using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using EM.IOC;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IFrame))]
    public class Frame : Group, IFrame
    {
        /// <inheritdoc/>
        public IView View { get; }
        /// <inheritdoc/>
        public ProjectionInfo Projection { get; set; }
        public Frame(int width, int height)
        {
            View = new View(this, width,  height);
            Children.CollectionChanged += Layers_CollectionChanged;
        }

        bool firstLayerAdded;
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
                            if (e.NewItems.Count > 0 && e.NewItems[0] is ILayer layer)
                            {
                                Projection = layer.DataSet?.Projection;
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
        public override void Draw(MapArgs mapArgs, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action? invalidateMapFrameAction = null)
        {
            if (mapArgs == null || mapArgs.Graphics == null || mapArgs.Bound.IsEmpty || mapArgs.Extent == null || mapArgs.Extent.IsEmpty() || mapArgs.DestExtent == null || mapArgs.DestExtent.IsEmpty() || cancelFunc?.Invoke() == true || Children.Count == 0)
            {
                return;
            }
            progressAction?.Invoke(string.Empty, 0);

            var visibleLayers = GetAllLayers().Where(x => x.GetVisible(mapArgs.DestExtent));
            var visibleLabelLayers = visibleLayers.Where(x => x is IFeatureLayer featureLayer && featureLayer.LabelLayer?.GetVisible(mapArgs.DestExtent) == true).Select(x => (x as IFeatureLayer).LabelLayer).ToList();
            var totalCount = visibleLayers.Count() + visibleLabelLayers.Count;//要素图层需要绘制标注，所以多算一次
            if (totalCount == 0)
            {
                return;
            }
            double layerPercent = visibleLayers.Count() / totalCount;//可见图层占比
            double labelLayerPercent = visibleLabelLayers.Count / totalCount;//标注图层占比
            #region 绘制图层
            Action<string, int> drawLayerProgressAction = (txt, progress) =>
            {
                if (progressAction != null)
                {
                    var destProgress = (int)(progress* layerPercent);
                    progressAction.Invoke(txt, destProgress);
                }
            };
            base.Draw(mapArgs, selected, drawLayerProgressAction, cancelFunc, invalidateMapFrameAction);//绘制图层
            #endregion

            #region 绘制标注
            Action<string, int> drawLabelProgressAction = (txt, progress) =>
            {
                if (progressAction != null)
                {
                    var destProgress = (int)(100.0 * layerPercent + progress / 100 * labelLayerPercent);
                    progressAction.Invoke(txt, destProgress);
                }
            };
            for (int i = visibleLabelLayers.Count - 1; i >= 0; i--)
            {
                if (cancelFunc?.Invoke() == true)
                {
                    break;
                }
                visibleLabelLayers[i].Draw(mapArgs, selected, drawLabelProgressAction, cancelFunc, invalidateMapFrameAction);
            }
            #endregion

            progressAction?.Invoke(string.Empty, 0);
        }

        /// <inheritdoc/>
        public IExtent GetMaxExtent(bool expand = false)
        {
            // to prevent exception when zoom to map with one layer with one point
            IExtent maxExtent = null;
            if (Extent == null)
            {
                return maxExtent;
            }
            const double Eps = 1e-7;
            maxExtent = Extent.Width < Eps || Extent.Height < Eps ? new Extent(Extent.MinX - Eps, Extent.MinY - Eps, Extent.MaxX + Eps, Extent.MaxY + Eps) : Extent.Copy();
            if (expand) maxExtent.ExpandBy(maxExtent.Width / 10, maxExtent.Height / 10);
            return maxExtent;
        }

        public void New()
        {
            ClearLayers();
            IsDirty = false;
        }

        public void Open(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string fileName)
        {
            throw new NotImplementedException();
        }

        //public bool ExtentsInitialized { get; set; }

        //public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.AntiAlias;
        private bool _isDirty;

        public event EventHandler FirstLayerAdded;

        /// <inheritdoc/>
        public bool IsDirty
        {
            get { return _isDirty; }
            protected set
            {
                _isDirty = value;
            }
        }

        public string FileName => throw new NotImplementedException();
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
