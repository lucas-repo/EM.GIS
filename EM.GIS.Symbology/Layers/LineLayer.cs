using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace EM.GIS.Symbology
{
    public class LineLayer : FeatureLayer, ILineLayer
    {
        public new ILineCategory DefaultCategory
        {
            get => base.DefaultCategory as ILineCategory;
            set => base.DefaultCategory = value;
        }
        public new ILineCategoryCollection Children { get => base.Children as ILineCategoryCollection; protected set => base.Children = value; }
        public LineLayer(IFeatureSet featureSet) : base(featureSet)
        {
            Children = new LineCategoryCollection(this);
            DefaultCategory = new LineCategory()
            {
                Text = "默认"
            };
        }

        protected override Rectangle DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            var ret = Rectangle.Empty;
            if (!(symbolizer is ILineSymbolizer lineSymbolizer))
            {
                return ret;
            }
            float scaleSize = (float)symbolizer.GetScale(proj);
            using (GraphicsPath path = new GraphicsPath())
            {
                GetLines(proj, geometry, path);
                lineSymbolizer.DrawLine(graphics, scaleSize, path);
                ret=path.GetBounds().ToRectangle();
            }
            return ret;
        }
        private void GetLines(IProj proj, IGeometry geometry, GraphicsPath path)
        {
            if (geometry.GeometryCount == 0)
            {
                PointF[] points = new PointF[geometry.CoordinateCount];
                for (int j = 0; j < geometry.CoordinateCount; j++)
                {
                    var coord = geometry.GetCoordinate(j);
                    PointF point = proj.ProjToPixelF(coord);
                    points[j] = point;
                }

                //去重
                var intPoints = DuplicationPreventer.Clean(points).ToArray();
                if (intPoints.Length >= 2)
                {
                    path.AddLines(intPoints);
                }
            }
            else
            {
                for (int i = 0; i < geometry.GeometryCount; i++)
                {
                    var partGeo = geometry.GetGeometry(i);
                    path.StartFigure();
                    GetLines(proj, partGeo, path);
                }
            }
        }
    }
}