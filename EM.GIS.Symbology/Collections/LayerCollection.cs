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
                IsVisible=true
            };
            return group;
        }
        private string GetDifferenctName(string prefix, int i=0)
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
            IFeatureLayer res = null;
            if (featureSet == null) return null;

            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                res = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polyline)
            {
                res = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                res = new PolygonLayer(featureSet);
            }

            if (res != null)
            {
                res.IsVisible = isVisible;
                res.ProgressHandler = ProgressHandler;
                Insert(0, res);
            }

            return res;
        }

        public IRasterLayer AddLayer(IRasterSet raster, bool isVisible = true)
        {
            IRasterLayer rasterLayer = null;
            if (raster != null)
            {
                rasterLayer = new RasterLayer(raster)
                {
                    IsVisible = isVisible,
                    ProgressHandler = ProgressHandler
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