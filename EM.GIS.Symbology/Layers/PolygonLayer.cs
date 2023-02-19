using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace EM.GIS.Symbology
{
    public class PolygonLayer : FeatureLayer, IPolygonLayer
    {
        public new IPolygonCategory DefaultCategory
        {
            get => base.DefaultCategory as IPolygonCategory;
            set => base.DefaultCategory = value;
        }

        public new IPolygonCategoryCollection Children { get => base.Children as IPolygonCategoryCollection; protected set => base.Children = value; }

        public PolygonLayer(IFeatureSet featureSet) : base(featureSet)
        {
            Children = new PolygonCategoryCollection(this);
            DefaultCategory = new PolygonCategory()
            {
                Text = "默认"
            };
        }
        protected override void DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            if (symbolizer is IPolygonSymbolizer polygonSymbolizer)
            {
                float scaleSize = (float)symbolizer.GetScale(proj);
                using (GraphicsPath path = new GraphicsPath())
                {
                    GetPolygons(proj, geometry, path);
                    polygonSymbolizer.DrawPolygon(graphics, scaleSize, path);
                }
            }
        }
        private void DrawGeometry(MapArgs drawArgs, Graphics context, float scaleSize, IPolygonSymbolizer polygonSymbolizer, IGeometry geometry)
        {
            if (geometry.GeometryCount == 0)
            {
                PointF[] points = new PointF[geometry.CoordinateCount];
                for (int j = 0; j < geometry.CoordinateCount; j++)
                {
                    var coord = geometry.GetCoordinate(j);
                    PointF point = drawArgs.ProjToPixelF(coord);
                    points[j] = point;
                }
                polygonSymbolizer.DrawPolygon(context, scaleSize, points.ToPath());
            }
            else
            {
                for (int i = 0; i < geometry.GeometryCount; i++)
                {
                    var partGeo = geometry.GetGeometry(i);
                    DrawGeometry(drawArgs, context, scaleSize, polygonSymbolizer, partGeo);
                }
            }
        }
        private void GetPolygons(IProj proj, IGeometry geometry, GraphicsPath path)
        {
            if (geometry.GeometryCount > 0)
            {
                for (int i = 0; i < geometry.GeometryCount; i++)
                {
                    var childGeo= geometry.GetGeometry(i);
                    GetPolygons(proj, childGeo, path);
                }
            }
            else
            {
                PointF[] points = new PointF[geometry.CoordinateCount];
                for (int i = 0; i < geometry.CoordinateCount; i++)
                {
                    var coord = geometry.GetCoordinate(i);
                    PointF point = proj.ProjToPixelF(coord);
                    points[i] = point;
                }
                //去重
                var intPoints = DuplicationPreventer.Clean(points).ToArray();
                if (intPoints.Length >= 2)
                {
                    path.AddLines(intPoints);
                }
            }
            //switch (geometry.GeometryType)
            //{
            //    case GeometryType.MultiPolygon:
            //        foreach (var partGeo in geometry.Geometries)
            //        {
            //            GetPolygons(proj, partGeo, path);
            //        }
            //        break;
            //    case GeometryType.Polygon:
            //        foreach (var partGeo in geometry.Geometries)
            //        {
            //            path.StartFigure();
            //            GetPolygons(proj, partGeo, path);
            //        }
            //        break;
            //    case GeometryType.LineString:
            //        PointF[] points = new PointF[geometry.CoordinateCount];
            //        for (int i = 0; i < geometry.CoordinateCount; i++)
            //        {
            //            var coord = geometry.GetCoordinate(i);
            //            PointF point = proj.ProjToPixelF(coord);
            //            points[i] = point;
            //        }
            //        //去重
            //        var intPoints = DuplicationPreventer.Clean(points).ToArray();
            //        if (intPoints.Length >= 2)
            //        {
            //            path.AddLines(intPoints);
            //        }

            //        break;
            //    default:
            //        throw new Exception("不支持的几何类型");
            //}
        }
    }
}