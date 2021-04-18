using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class PolygonLayer : FeatureLayer, IPolygonLayer
    {
        public new IPolygonCategory DefaultCategory
        {
            get => base.DefaultCategory as IPolygonCategory;
            set => base.DefaultCategory = value;
        }

        public new IPolygonCategoryCollection Categories { get=> LegendItems as IPolygonCategoryCollection; }

        public PolygonLayer(IFeatureSet featureSet) : base(featureSet)
        {
            LegendItems = new PolygonCategoryCollection(this);
            DefaultCategory = new PolygonCategory()
            {
                Text = "默认"
            };
        }
        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (drawArgs == null || !(symbolizer is IPolygonSymbolizer polygonSymbolizer) || geometry == null)
            {
                return;
            }
            float scaleSize = (float)symbolizer.GetScale(drawArgs);
            using (GraphicsPath path = new GraphicsPath())
            {
                GetPolygons(drawArgs, geometry, path);
                polygonSymbolizer.DrawPolygon(drawArgs.Device, scaleSize, path);
            }
        }
        private void DrawGeometry(MapArgs drawArgs, Graphics context, float scaleSize, IPolygonSymbolizer polygonSymbolizer, IGeometry geometry)
        {
            int geoCount = geometry.Geometries.Count;
            if (geoCount == 0)
            {
                int pointCount = geometry.Coordinates.Count;
                PointF[] points = new PointF[pointCount];
                for (int j = 0; j < pointCount; j++)
                {
                    var coord = geometry.Coordinates[j];
                    PointF point = drawArgs.CoordinateToPointF(coord);
                    points[j] = point;
                }
                polygonSymbolizer.DrawPolygon(context, scaleSize, points.ToPath());
            }
            else
            {
                for (int i = 0; i < geoCount; i++)
                {
                    var partGeo = geometry.Geometries[i];
                    DrawGeometry(drawArgs, context, scaleSize, polygonSymbolizer, partGeo);
                }
            }
        }
        private void GetPolygons(MapArgs drawArgs, IGeometry geometry, GraphicsPath path)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.MultiPolygon:
                    foreach (var partGeo in geometry.Geometries)
                    {
                        GetPolygons(drawArgs, partGeo, path);
                    }
                    break;
                case GeometryType.Polygon:
                    foreach (var partGeo in geometry.Geometries)
                    {
                        path.StartFigure();
                        GetPolygons(drawArgs, partGeo, path);
                    }
                    break;
                case GeometryType.LineString:
                    int pointCount = geometry.Coordinates.Count;
                    PointF[] points = new PointF[pointCount];
                    for (int j = 0; j < pointCount; j++)
                    {
                        var coord = geometry.Coordinates[j];
                        PointF point = drawArgs.CoordinateToPointF(coord);
                        points[j] = point;
                    }
                    path.AddLines(points);
                    break;
                default:
                    throw new Exception("不支持的几何类型");
            }
        }
    }
}