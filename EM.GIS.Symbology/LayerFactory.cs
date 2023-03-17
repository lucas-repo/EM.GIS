using BruTile.Wms;
using EM.GIS.Data;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(ILayerFactory))]
    public class LayerFactory : ILayerFactory
    {
        /// <inheritdoc/>
        public IFeatureLayer? CreateFeatureLayer(string name, FeatureType featureType)
        {
            IFeatureLayer? ret=null;
            var dataSetFactory = IocManager.Default.GetService<IDataSetFactory>();
            if (dataSetFactory == null)
            {
                return ret;
            }
            var featureSet = dataSetFactory.CreateFeatureSet(name, featureType);
            if(featureSet==null)
            {
                return ret;
            }
            switch (featureType)
            {
                case FeatureType.Point:
                case FeatureType.MultiPoint:
                    ret = new PointLayer(featureSet);
                    break;
                case FeatureType.Polyline:
                    ret = new LineLayer(featureSet);
                    break;
                case FeatureType.Polygon:
                    ret = new PolygonLayer(featureSet);
                    break;
            }
            return ret;
        }
    }
}
