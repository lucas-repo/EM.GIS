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

        protected override Rectangle DrawGeometry(IProj proj, Graphics graphics, IFeatureSymbolizer symbolizer, IGeometry geometry)
        {
            var ret = Rectangle.Empty;
            if (symbolizer is IPointSymbolizer pointSymbolizer)
            {
                ret = DrawPoint(proj, graphics, pointSymbolizer, geometry);
            }
            return ret;
        }
        private Rectangle DrawPoint(IProj proj, Graphics graphics, IPointSymbolizer symbolizer, IGeometry geometry)
        {
            var ret = Rectangle.Empty;
            if (geometry.GeometryCount == 0)
            {
                float scaleSize = (float)symbolizer.GetScale(proj);
                for (int i = 0; i < geometry.CoordinateCount; i++)
                {
                    var coord = geometry.GetCoordinate(i);
                    PointF point = proj.ProjToPixelF(coord);
                    symbolizer.DrawPoint(graphics, scaleSize, point);
                    var pointRect = new RectangleF(point.X - symbolizer.Size.Width / 2, point.Y - symbolizer.Size.Height / 2, symbolizer.Size.Width, symbolizer.Size.Height).ToRectangle();
                    ret = ret.ExpandToInclude(pointRect);
                }
            }
            else
            {
                for (int i = 0; i < geometry.GeometryCount; i++)
                {
                    var partGeo = geometry.GetGeometry(i);
                    var pointRect = DrawPoint(proj, graphics, symbolizer, partGeo);
                    ret = ret.ExpandToInclude(pointRect);
                }
            }
            return ret;
        }
    }
}