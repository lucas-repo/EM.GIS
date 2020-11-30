using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class LineLayer : FeatureLayer, ILineLayer
    {
        public new ILineCategory DefaultCategory
        {
            get => base.DefaultCategory as ILineCategory;
            set => base.DefaultCategory = value;
        }
        public new ILineCategoryCollection Categories { get => Items as ILineCategoryCollection; }
        public LineLayer(IFeatureSet featureSet) : base(featureSet)
        {
            DefaultCategory = new LineCategory();
            Items = new LineCategoryCollection(this)
            {
                DefaultCategory
            };
        }

        protected override void DrawGeometry(MapArgs drawArgs, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (drawArgs == null || !(symbolizer is ILineSymbolizer lineSymbolizer) || geometry == null)
            {
                return;
            }
            float scaleSize = (float)symbolizer.GetScale(drawArgs);
            using (GraphicsPath path = new GraphicsPath())
            {
                GetLines(drawArgs, geometry, path);
                lineSymbolizer.DrawLine(drawArgs.Device, scaleSize, path);
            }
        }
        private void GetLines(MapArgs drawArgs, IGeometry geometry, GraphicsPath path)
        {
            switch (geometry.GeometryType)
            {
                case GeometryType.MultiLineString:
                    int geoCount = geometry.GeometryCount;
                    for (int i = 0; i < geoCount; i++)
                    {
                        var partGeo = geometry.GetGeometry(i);
                        path.StartFigure();
                        GetLines(drawArgs, partGeo, path);
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