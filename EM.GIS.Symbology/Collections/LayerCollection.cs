using EM.GIS.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层集合
    /// </summary>
    public class LayerCollection : LegendItemCollection, ILayerCollection
    {
        #region 重写部分
        public new ILayer this[int index] { get => Items[index] as ILayer; set => Items[index] = value; }
        public new IGroup Parent { get => base.Parent as IGroup; set => base.Parent = value; }

        #endregion
        public IProgressHandler ProgressHandler { get; set; }


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

            featureSet.ProgressHandler = ProgressHandler;
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
                base.Add(res);
            }

            return res;
        }

        public IRasterLayer AddLayer(IRasterSet raster, bool isVisible = true)
        {
            IRasterLayer rasterLayer = null;
            if (raster != null)
            {
                raster.ProgressHandler = ProgressHandler;
                rasterLayer = new RasterLayer(raster)
                {
                    IsVisible = isVisible
                };
                Add(rasterLayer);
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