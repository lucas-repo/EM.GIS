using EM.Bases;
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
    public class Group : LegendItem, IGroup
    {
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
                            extent=layer.Extent;
                            break;
                        case IGroup group:
                            extent=group.Extent;
                            break;
                    }
                    if (extent==null)
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
        }
        public void Draw(Graphics graphics, Rectangle rectangle, IExtent extent, bool selected = false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction = null)
        {
            if (graphics == null || rectangle.Width * rectangle.Height == 0 || extent == null || extent.Width * extent.Height == 0 || cancelFunc?.Invoke() == true)
            {
                return;
            }
            
            Progress?.Invoke(0, String.Empty);
            for (int i = Children.Count() - 1; i >= 0; i--)
            {
                if (Children.ElementAt(i) is ILayer layer && layer.GetVisible(extent, rectangle))
                {
                    layer.Draw(graphics, rectangle, extent, selected, cancelFunc, invalidateMapFrameAction);
                    invalidateMapFrameAction?.Invoke();
                }
            }
            Progress?.Invoke(100, String.Empty);
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
                AddLayer(featureLayer, index);
            }
            return featureLayer;
        }

        public IRasterLayer AddLayer(IRasterSet rasterSet, int? index = null)
        {
            IRasterLayer rasterLayer = null;
            if (rasterSet != null)
            {
                rasterLayer = new RasterLayer(rasterSet);
                AddLayer(rasterLayer, index);
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
            Children.RemoveAt(index);
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
