using EM.GIS.Data;
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
        public new IEnumerator<ILayer> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as ILayer;
            }
        }

        #endregion
        public IProgressHandler ProgressHandler { get; set; }


        public ILayer AddLayer(IDataSet dataSet)
        {
            ILayer layer = null;
            //var ss = dataSet as ISelfLoadSet;
            //if (ss != null) return Add(ss);

            if (dataSet is IFeatureSet fs)
            {
                layer = AddLayer(fs);
            }
            else if (dataSet is IRasterSet r)
            {
                layer = AddLayer(r);
            }
            return layer;

            //var id = dataSet as IImageData;
            //return id != null ? Add(id) : null;
        }

        public IFeatureLayer AddLayer(IFeatureSet featureSet)
        {
            IFeatureLayer res = null;
            if (featureSet == null) return null;

            featureSet.ProgressHandler = ProgressHandler;
            if (featureSet.FeatureType == FeatureType.Point || featureSet.FeatureType == FeatureType.MultiPoint)
            {
                res = new PointLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Line)
            {
                res = new LineLayer(featureSet);
            }
            else if (featureSet.FeatureType == FeatureType.Polygon)
            {
                res = new PolygonLayer(featureSet);
            }

            if (res != null)
            {
                base.Add(res);
                res.ProgressHandler = ProgressHandler;
            }

            return res;
        }

        public IRasterLayer AddLayer(IRasterSet raster)
        {
            IRasterLayer rasterLayer = null;
            if (raster != null)
            {
                raster.ProgressHandler = ProgressHandler;
                rasterLayer = new RasterLayer(raster);
                Add(rasterLayer);
            }
            return rasterLayer;
        }

        public ILayer AddLayer(string path)
        {
            IDataSet dataSet = DataFactory.Default.DriverFactory.Open(path);
            return AddLayer(dataSet);
        }

    }
}