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
        public new IPointCategoryCollection Children { get => base.Children as IPointCategoryCollection; protected set => base.Children = value; }
        public PointLayer(IFeatureSet featureSet) : base(featureSet)
        {
            Children = new PointCategoryCollection(this);
            DefaultCategory = new PointCategory()
            {
                Text = "默认"
            };
        }

        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            DrawPoint(drawArgs, symbolizer as IPointSymbolizer, geometry);
        }
        private void DrawPoint(MapArgs drawArgs, IPointSymbolizer symbolizer, IGeometry geometry)
        {
            if (geometry.Geometries.Count == 0)
            {
                float scaleSize = (float)symbolizer.GetScale(drawArgs);
                int pointCount = geometry.Coordinates.Count;
                for (int j = 0; j < pointCount; j++)
                {
                    var coord = geometry.Coordinates[j];
                    PointF point = drawArgs.CoordinateToPointF(coord);
                    symbolizer.DrawPoint(drawArgs.Device, scaleSize, point);
                }
            }
            else
            {
                int geoCount = geometry.Geometries.Count;
                for (int i = 0; i < geoCount; i++)
                {
                    var partGeo = geometry.Geometries[i];
                    DrawPoint(drawArgs, symbolizer, partGeo);
                }
            }

        }
    }
}