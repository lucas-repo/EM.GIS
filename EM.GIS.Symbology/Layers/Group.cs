using EM.GIS.Data;
using EM.GIS.Geometries;
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
    public class Group : Layer, IGroup
    {
        public ILayerCollection Layers { get => LegendItems as ILayerCollection; }
        public int LayerCount => GetLayers().Count();
        public Group()
        {
            LegendItems = new LayerCollection();
            LegendItems.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IExtent extent = Extent.Copy();
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (extent == null)
                    {
                        extent = new Extent();
                    }
                    foreach (ILayer item in e.NewItems)
                    {
                        extent.ExpandToInclude(item.Extent);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    extent = new Extent();
                    foreach (var layer in GetAllLayers())
                    {
                        extent.ExpandToInclude(layer.Extent);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    return;
                case NotifyCollectionChangedAction.Reset:
                    extent = new Extent();
                    break;
            }
            Extent = extent;
        }

        protected override void OnDraw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null)
        {
            var visibleLayers = GetLayers().Where(x => x.GetVisible(extent, rectangle));
            foreach (var layer in visibleLayers)
            {
                layer?.Draw(graphics, rectangle, extent, selected, cancelFunc);
            }
        }

        public ILayer GetLayer(int index)
        {
            ILayer layer = null;
            if (index >= 0 && index < LayerCount)
            {
                layer = GetLayers().ElementAt(index);
            }
            return layer;
        }

        public IEnumerable<ILayer> GetLayers()
        {
            foreach (ILayer item in LegendItems)
            {
                yield return item;
            }
        }

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
        public IEnumerable<IRasterLayer> GetAllRasterLayers()
        {
            return GetLayers<IRasterLayer>(GetLayers(), true);
        }

        public bool AddLayer(ILayer layer, int? index = null)
        {
            bool ret = false;
            if (layer == null)
            {
                return ret;
            }
            if (index.HasValue)
            {
                if (index.Value < 0 || index.Value > LegendItems.Count)
                {
                    return ret;
                }
                LegendItems.Insert(index.Value, layer);
            }
            else
            {
                LegendItems.Add(layer);
            }
            if (layer.ProgressHandler == null && ProgressHandler != null)
            {
                layer.ProgressHandler = ProgressHandler;
            }
            ret = true;
            return ret;
        }

        public ILayer AddLayer(string filename, int? index = null)
        {
            IDataSet dataSet = DataFactory.Default.DriverFactory.Open(filename);
            return AddLayer(dataSet, index);
        }

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

        public IFeatureLayer AddLayer(IFeatureSet featureSet, int? index = null)
        {
            IFeatureLayer featureLayer = null;
            if (featureSet == null) return null;

            featureSet.ProgressHandler = ProgressHandler;
            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                featureLayer = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polyline)
            {
                featureLayer = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                featureLayer = new PolygonLayer(featureSet);
            }

            if (featureLayer != null)
            {
                if (AddLayer(featureLayer, index))
                {
                    if (featureSet.ProgressHandler == null && ProgressHandler != null)
                    {
                        featureSet.ProgressHandler = ProgressHandler;
                    }
                }
            }
            return featureLayer;
        }

        public IRasterLayer AddLayer(IRasterSet rasterSet, int? index = null)
        {
            IRasterLayer rasterLayer = null;
            if (rasterSet != null)
            {
                rasterSet.ProgressHandler = ProgressHandler;
                rasterLayer = new RasterLayer(rasterSet);
                if (AddLayer(rasterLayer, index))
                {
                    if (rasterSet.ProgressHandler == null && ProgressHandler != null)
                    {
                        rasterSet.ProgressHandler = ProgressHandler;
                    }
                }
            }
            return rasterLayer;
        }

        public IEnumerable<ILayer> GetAllLayers()
        {
            return GetLayers<ILayer>(GetLayers(), true);
        }

        public bool RemoveLayer(ILayer layer)
        {
            return LegendItems.Remove(layer);
        }

        public void RemoveLayerAt(int index)
        {
            LegendItems.RemoveAt(index);
        }

        public void ClearLayers()
        {
            LegendItems.Clear();
        }

        public IEnumerable<IFeatureLayer> GetFeatureLayers()
        {
            return GetLayers<IFeatureLayer>(GetLayers(), false);
        }

        public IEnumerable<IRasterLayer> GetRasterLayers()
        {
            return GetLayers<IRasterLayer>(GetLayers(), false);
        }

    }
}
