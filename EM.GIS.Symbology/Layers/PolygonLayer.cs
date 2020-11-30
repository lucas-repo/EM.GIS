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

        public new IPolygonCategoryCollection Categories { get=> Items as IPolygonCategoryCollection; }

        public PolygonLayer(IFeatureSet featureSet) : base(featureSet)
        {
            DefaultCategory = new PolygonCategory();
            Items = new PolygonCategoryCollection(this)
            {
                DefaultCategory
            };
        }
        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (drawArgs == null || !(symbolizer is IPolygonSymbolizer polygonSymbolizer) || geometry == null)
            {
                return;
            }
            float scaleSize = (float)symbolizer.GetScale(drawArgs);
            GraphicsPath path = new GraphicsPath();
            GetPolygons(drawArgs, geometry, path);
            polygonSymbolizer.DrawPolygon(drawArgs.Device, scaleSize, path);
        }
        private void DrawGeometry(MapArgs drawArgs, Graphics context, float scaleSize, IPolygonSymbolizer polygonSymbolizer, IGeometry geometry)
        {
            int geoCount = geometry.GeometryCount;
            if (geoCount == 0)
            {
                int pointCount = geometry.PointCount;
                PointF[] points = new PointF[pointCount];
                for (int j = 0; j < pointCount; j++)
                {
                    var coord = geometry.GetCoord(j);
                    PointF point = drawArgs.ProjToPixelPointF(coord);
                    points[j] = point;
                }
                polygonSymbolizer.DrawPolygon(context, scaleSize, points.ToPath());
            }
            else
            {
                for (int i = 0; i < geoCount; i++)
                {
                    var partGeo = geometry.GetGeometry(i);
                    DrawGeometry(drawArgs, context, scaleSize, polygonSymbolizer, partGeo);
                }
            }
        }
        private void GetPolygons(MapArgs drawArgs, IGeometry geometry, GraphicsPath path)
        {
            int geoCount = geometry.GeometryCount;
            switch (geometry.GeometryType)
            {
                case GeometryType.MultiPolygon:
                    for (int i = 0; i < geoCount; i++)
                    {
                        var partGeo = geometry.GetGeometry(i);
                        GetPolygons(drawArgs, partGeo, path);
                    }
                    break;
                case GeometryType.Polygon:
                    for (int i = 0; i < geoCount; i++)
                    {
                        var partGeo = geometry.GetGeometry(i);
                        path.StartFigure();
                        GetPolygons(drawArgs, partGeo, path);
                    }
                    break;
                case GeometryType.LineString:
                    int pointCount = geometry.PointCount;
                    PointF[] points = new PointF[pointCount];
                    for (int j = 0; j < pointCount; j++)
                    {
                        var coord = geometry.GetCoord(j);
                        PointF point = drawArgs.ProjToPixelPointF(coord);
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