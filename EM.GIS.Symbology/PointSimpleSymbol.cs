using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public class PointSimpleSymbol : PointSymbol, IPointSimpleSymbol
    {
        public PointSimpleSymbol() : base(PointSymbolType.Simple)
        {
            PointShape = PointShape.Ellipse;
        }
        public PointSimpleSymbol(Color color)
           : this()
        {
            Color = color;
        }
        public PointSimpleSymbol(Color color, PointShape shape) : this(color)
        {
            PointShape = shape;
        }

        public PointSimpleSymbol(Color color, PointShape shape, float size) : this(color, shape)
        {
            Size = new SizeF(size, size);
        }

        public PointShape PointShape { get; set; }

        protected override void OnDrawPoint(Graphics g, float scale)
        {
            float width = scale * Size.Width;
            float height = scale * Size.Height;
            SizeF size = new SizeF(width, height);
            using (GraphicsPath gp = new GraphicsPath())
            {
                switch (PointShape)
                {
                    case PointShape.Diamond:
                        PathHelper.AddRegularPoly(gp, size, 4);
                        break;
                    case PointShape.Ellipse:
                        PathHelper.AddEllipse(gp, size);
                        break;
                    case PointShape.Hexagon:
                        PathHelper.AddRegularPoly(gp, size, 6);
                        break;
                    case PointShape.Pentagon:
                        PathHelper.AddRegularPoly(gp, size, 5);
                        break;
                    case PointShape.Rectangle:
                        gp.AddRectangle(new RectangleF(-size.Width / 2, -size.Height / 2, size.Width, size.Height));
                        break;
                    case PointShape.Star:
                        PathHelper.AddStar(gp, size);
                        break;
                    case PointShape.Triangle:
                        PathHelper.AddRegularPoly(gp, size, 3);
                        break;
                }
                using (Brush brush = new SolidBrush(Color))
                {
                    g.FillPath(brush, gp);
                }
                DrawOutLine(g, scale, gp);
            }
        }
    }
}
