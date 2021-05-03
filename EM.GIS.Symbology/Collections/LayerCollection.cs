using EM.GIS.Data;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合
    /// </summary>
    public class LayerCollection : LegendItemCollection, ILayerCollection
    {
        public LayerCollection(IGroup parent) : base(parent)
        {
        }

        public new ILayer this[int index] { get => Items[index] as ILayer; set => Items[index] = value; }
        public new IGroup Parent { get => base.Parent as IGroup; set => base.Parent = value; }

        public IGroup AddGroup(string groupName = null)
        {
            string destGroupName = groupName;
            if (string.IsNullOrEmpty(groupName))
            {
                destGroupName = GetDifferenctName("分组");
            }
            IGroup group = new Group()
            {
                Text = destGroupName,
                IsVisible = true,
                Parent = Parent
            };
            Insert(0, group);
            return group;
        }
        private string GetDifferenctName(string prefix, int i = 0)
        {
            string name = $"{prefix}{i}";
            foreach (ILegendItem item in this)
            {
                if (item.Text == name)
                {
                    name = GetDifferenctName(prefix, ++i);
                    break;
                }
            }
            return name;
        }
        public ILayer AddLayer(IDataSet dataSet, bool isVisible = true)
        {
            ILayer layer = null;
            //var ss = dataSet as ISelfLoadSet;
            //if (ss != null) return Add(ss);

            if (dataSet is IFeatureSet fs)
            {
                layer = AddLayer(fs, isVisible);
            }
            else if (dataSet is IRasterSet r)
            {
                layer = AddLayer(r, isVisible);
            }
            if (dataSet != null)
            {
                layer.Text = dataSet.Name;
            }
            return layer;
        }

        public IFeatureLayer AddLayer(IFeatureSet featureSet, bool isVisible = true)
        {
            IFeatureLayer layer = null;
            if (featureSet == null) return null;

            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                layer = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polyline)
            {
                layer = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                layer = new PolygonLayer(featureSet);
            }

            if (layer != null)
            {
                layer.IsVisible = isVisible;
                layer.ProgressHandler = ProgressHandler;
                layer.Parent = Parent;
                Insert(0, layer);
            }

            return layer;
        }

        public IRasterLayer AddLayer(IRasterSet raster, bool isVisible = true)
        {
            IRasterLayer rasterLayer = null;
            if (raster != null)
            {
                rasterLayer = new RasterLayer(raster)
                {
                    IsVisible = isVisible,
                    ProgressHandler = ProgressHandler,
                    Parent = Parent
                };
                Insert(0, rasterLayer);
            }
            return rasterLayer;
        }

        public ILayer AddLayer(string path, bool isVisible = true)
        {
            IDataSet dataSet = DataFactory.Default.DriverFactory.Open(path);
            return AddLayer(dataSet, isVisible);
        }
    }
}