using EM.GIS.Data;

namespace EM.GIS.Symbology
{
    public class LayerManager : ILayerManager
    {
        private static ILayerManager _default;

        public static ILayerManager Default
        {
            get
            {
                if (_default == null)
                {
                    _default = new LayerManager();
                }
                return _default;
            }
        }

        public virtual ILayer OpenLayer(string dataPath)
        {
            ILayer layer = null;
            var dataset = DataFactory.Default.DriverFactory.Open(dataPath);
            if ( dataset is IRasterSet rasterSet)
            {
                layer = new RasterLayer(rasterSet);
            }
            else if (dataset is IFeatureSet featureSet)
            {
                switch (featureSet.FeatureType)
                {
                    case FeatureType.Point:
                        layer = new PointLayer(featureSet);
                        break;
                    case FeatureType.Line:
                        layer = new LineLayer(featureSet);
                        break;
                    case FeatureType.Polygon:
                        layer = new PolygonLayer(featureSet);
                        break;
                }
            }
            return layer;
        }

    }
}
