using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public abstract class PolygonSymbol : OutLineSymbol, IPolygonSymbol
    {
        public PolygonSymbolType PolygonSymbolType { get; }

        public RectangleF Bounds { get; set; }
        public PolygonSymbol(PolygonSymbolType polygonSymbolType)
        {
            PolygonSymbolType = polygonSymbolType;
        }

        public void DrawPolygon(Graphics context, float scale, GraphicsPath path)
        {
            if (context == null || path == null)
            {
                return;
            }
            using (Brush brush = GetBrush())
            {
                if (brush == null)
                {
                    return;
                }
                context.FillPath(brush, path);
            }
            DrawOutLine(context, scale, path);
        }

        public virtual Brush GetBrush()
        {
            Brush brush = new SolidBrush(Color);
            return brush;
        }
    }
}
