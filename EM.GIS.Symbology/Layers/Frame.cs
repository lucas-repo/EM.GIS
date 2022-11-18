using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架
    /// </summary>
    public class Frame : Group, IFrame
    {
        /// <inheritdoc/>
        public IView MapView { get; }
        public Frame(int width, int height)
        {
            MapView = new View(this, width, height);
            DrawingLayers = new LayerCollection(this, this);
            Children.CollectionChanged += Layers_CollectionChanged;
        }

        bool firstLayerAdded;
        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!firstLayerAdded)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count > 0)
                {
                    firstLayerAdded = true;
                }
                if (firstLayerAdded)
                {
                    var extent = Extent.Copy();
                    if (!extent.IsEmpty())
                    {
                        extent.ExpandBy(extent.Width / 10, extent.Height / 10);
                    }
                    MapView.Extent = extent;
                    return;
                }
            }
            MapView.ResetBuffer();
        }

        /// <inheritdoc/>
        public override void Draw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            base.Draw(graphics, rectangle, extent, selected, cancelFunc, invalidateMapFrameAction);
            var visibleDrawingFeatureLayers = new List<IFeatureLayer>();
            if (DrawingLayers != null)
            {
                foreach (ILayer item in DrawingLayers)
                {
                    if (cancelFunc?.Invoke() == true)
                    {
                        break;
                    }
                    if (item.GetVisible(extent, rectangle))
                    {
                        item.Draw(graphics, rectangle, extent, selected, cancelFunc);
                        if (item is IFeatureLayer featureLayer)
                        {
                            visibleDrawingFeatureLayers.Add(featureLayer);
                        }
                    }
                }
            }

            var featureLayers = GetFeatureLayers().Where(x => x.GetVisible(extent, rectangle)).Union(visibleDrawingFeatureLayers);
            var labelLayers = featureLayers.Where(x => x.LabelLayer?.GetVisible(extent, rectangle) == true).Select(x => x.LabelLayer);
            foreach (var layer in labelLayers)
            {
                if (cancelFunc?.Invoke()== true)
                {
                    break;
                }
                layer.Draw(graphics, rectangle, extent, selected, cancelFunc);
            }
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

        //public bool ExtentsInitialized { get; set; }

        //public SmoothingMode SmoothingMode { get; set; } = SmoothingMode.AntiAlias;
        /// <inheritdoc/>
        public ILayerCollection DrawingLayers { get; }

    }
}
