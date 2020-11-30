using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public abstract class PointSymbol : OutLineSymbol, IPointSymbol
    {
        public float Angle { get; set; }
        public SizeF Size { get; set; }
        public PointF Offset { get; set; }
        public PointSymbolType PointSymbolType { get; }
        public PointSymbol(PointSymbolType pointSymbolType)
        {
            PointSymbolType = pointSymbolType;
        }
        public void DrawPoint(Graphics g, float scale, PointF point)
        {
            if (g != null && scale != 0 && !Size.IsEmpty)
            {
                Matrix old = g.Transform;
                Matrix adjust = g.Transform;
                float dx = point.X + scale * Offset.X;
                float dy = point.Y - scale * Offset.Y;
                adjust.Translate(dx, dy);
                adjust.Rotate(Angle);
                g.Transform = adjust;

                OnDrawPoint(g, scale);

                g.Transform = old;
            }
        }

        protected abstract void OnDrawPoint(Graphics g, float scale);
    }
}
