using BruTile.Wms;
using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;


namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分组
    /// </summary>
    public class Group : LegendItem, IGroup
    {
        public new IGroup Parent
        {
            get => base.Parent as IGroup;
            set => base.Parent = value;
        }
        public new ILayerCollection Children
        {
            get => base.Children as ILayerCollection;
            set => base.Children = value;
        }
        public int LayerCount => Children.Count();

        public IExtent Extent
        {
            get
            {
                IExtent destExtent = new Extent();
                foreach (var item in Children)
                {
                    IExtent extent = null;
                    switch (item)
                    {
                        case ILayer layer:
                            extent = layer.Extent;
                            break;
                        case IGroup group:
                            extent = group.Extent;
                            break;
                    }
                    if (extent == null)
                    {
                        continue;
                    }
                    destExtent.ExpandToInclude(extent);
                }
                return destExtent;
            }
        }

        public bool UseDynamicVisibility { get; set; }
        public double MaxInverseScale { get; set; }
        public double MinInverseScale { get; set; }
        public Group()
        {
            Children = new LayerCollection(this);
        }
        /// <inheritdoc/>
        public virtual void Draw(Graphics graphics, ProjectionInfo projection, Rectangle rectangle, IExtent extent, bool selected = false, Action<string, int> progressAction = null, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            if (graphics == null || rectangle.IsEmpty || extent == null || extent.IsEmpty() || cancelFunc?.Invoke() == true || Children.Count == 0)
            {
                return;
            }
            string progressStr = this.GetProgressString();
            progressAction?.Invoke(progressStr, 0);
            double increment = 100.0 / Children.Count();
            int totalProgress = 0;
            Action<string, int> newProgressAction = (txt, progress) =>
            {
                if (progressAction != null)
                {
                    var destProgress = (int)((double)progress / Children.Count + totalProgress);
                    progressAction.Invoke(txt, destProgress);
                }
            };
            for (int i = Children.Count-1; i >=0; i--)
            {
                var item = Children[i];
                if (item is ILegendItem legendItem && legendItem.IsVisible)
                {
                    if (item is ILayer layer)
                    {
                        layer.Draw(graphics, projection, rectangle, extent, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                    }
                    else if (item is IGroup group)
                    {
                        group.Draw(graphics, projection, rectangle, extent, selected, newProgressAction, cancelFunc, invalidateMapFrameAction);
                    }
                }
                totalProgress += (int)increment;
                invalidateMapFrameAction?.Invoke();
            }
            progressAction?.Invoke(progressStr, 100);
        }
        /// <inheritdoc/>
        public ILayer GetLayer(int index)
        {
            ILayer layer = null;
            if (index >= 0 && index < LayerCount)
            {
                layer = GetLayers().ElementAt(index);
            }
            return layer;
        }

        /// <inheritdoc/>
        public IEnumerable<ILayer> GetLayers()
        {
            foreach (ILayer item in Children)
            {
                yield return item;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IFeatureLayer> GetAllFeatureLayers()
        {
            return GetLayers<IFeatureLayer>(GetLayers(), true);
        }
        /// <summary>
        /// 从指定图层集合获取指定类型的图层
        /// </summary>
        /// <typeparam name="T">图层类型</typeparam>
        /// <param name="layers">图层集合</param>
        /// <param name="searchChildren">是否查询子图层</param>
        /// <returns></returns>
        private IEnumerable<T> GetLayers<T>(IEnumerable<ILayer> layers, bool searchChildren) where T : ILayer
        {
            foreach (var layer in layers)
            {
                if (layer is IGroup group)
                {
                    if (searchChildren)
                    {
                        foreach (var item in GetLayers<T>(group.GetLayers(), searchChildren))
                        {
                            yield return item;
                        }
                    }
                }
                else if (layer is T t)
                {
                    yield return t;
                }
            }
        }
        /// <inheritdoc/>
        public IEnumerable<IRasterLayer> GetAllRasterLayers()
        {
            return GetLayers<IRasterLayer>(GetLayers(), true);
        }

        /// <inheritdoc/>
        public bool AddLayer(ILayer layer, int? index = null)
        {
            bool ret = false;
            if (layer == null)
            {
                return ret;
            }
            if (index.HasValue)
            {
                if (index.Value < 0 || index.Value > Children.Count)
                {
                    return ret;
                }
                Children.Insert(index.Value, layer);
            }
            else
            {
                Children.Add(layer);
            }
            ret = true;
            return ret;
        }

        /// <inheritdoc/>
        public ILayer AddLayer(IDataSet dataSet, int? index = null)
        {
            ILayer layer = null;
            if (dataSet is IFeatureSet featureSet)
            {
                layer = AddLayer(featureSet, index);
            }
            else if (dataSet is IRasterSet rasterSet)
            {
                layer = AddLayer(rasterSet, index);
            }
            return layer;
        }

        /// <inheritdoc/>
        public IFeatureLayer AddLayer(IFeatureSet featureSet, int? index = null)
        {
            IFeatureLayer featureLayer = null;
            if (featureSet == null) return featureLayer;
            switch (featureSet.FeatureType)
            {
                case FeatureType.Point:
                case FeatureType.MultiPoint:
                    featureLayer = new PointLayer(featureSet);
                    break;
                case FeatureType.Polyline:
                    featureLayer = new LineLayer(featureSet);
                    break;
                case FeatureType.Polygon:
                    featureLayer = new PolygonLayer(featureSet);
                    break;
                default:
                    return featureLayer;
            }
            featureLayer.Text = featureSet.Name;
            if (featureLayer != null)
            {
                AddLayer(featureLayer, index);
            }
            return featureLayer;
        }

        /// <inheritdoc/>
        public IRasterLayer AddLayer(IRasterSet rasterSet, int? index = null)
        {
            IRasterLayer rasterLayer = null;
            if (rasterSet != null)
            {
                rasterLayer = new RasterLayer(rasterSet)
                {
                    Text = rasterSet.Name
                };
                AddLayer(rasterLayer, index);
            }
            return rasterLayer;
        }

        /// <inheritdoc/>
        public IEnumerable<ILayer> GetAllLayers()
        {
            return GetLayers<ILayer>(GetLayers(), true);
        }

        /// <inheritdoc/>
        public bool RemoveLayer(ILayer layer)
        {
            return Children.Remove(layer);
        }

        /// <inheritdoc/>
        public void RemoveLayerAt(int index)
        {
            Children.RemoveAt(index);
        }

        /// <inheritdoc/>
        public void ClearLayers()
        {
            if (Children.Count > 0)
            {
                Children.Clear();
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IFeatureLayer> GetFeatureLayers()
        {
            return GetLayers<IFeatureLayer>(GetLayers(), false);
        }

        /// <inheritdoc/>
        public IEnumerable<IRasterLayer> GetRasterLayers()
        {
            return GetLayers<IRasterLayer>(GetLayers(), false);
        }
    }
}
