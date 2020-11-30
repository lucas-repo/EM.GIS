using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Drawing;

namespace EM.GIS.Symbology
{
    public class PointLayer : FeatureLayer, IPointLayer
    {
        public new IPointCategory DefaultCategory
        {
            get => base.DefaultCategory as IPointCategory;
            set => base.DefaultCategory = value;
        }
        public new IPointCategoryCollection Categories { get => Items as IPointCategoryCollection; }
        public PointLayer(IFeatureSet featureSet) : base(featureSet)
        {
            DefaultCategory = new PointCategory();
            Items = new PointCategoryCollection(this)
            { 
                DefaultCategory
            };
        }

        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            int geometryCount = geometry.GeometryCount; 
            for (int i = 0; i < geometryCount; i++)
            {
                var partGeo = geometry.GetGeometry(i);
                int pointCount = partGeo.PointCount;
                float scaleSize = (float)symbolizer.GetScale(drawArgs);
                for (int j = 0; j < pointCount; j++)
                {
                    var coord= partGeo.GetCoord(j);
                    PointF point = drawArgs.ProjToPixelPointF(coord);
                    (symbolizer as IPointSymbolizer).DrawPoint(drawArgs.Device, scaleSize, point);
                }
            }
        }
    }
}