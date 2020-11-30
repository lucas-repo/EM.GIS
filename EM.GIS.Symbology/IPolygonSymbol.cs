using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface IPolygonSymbol: IFeatureSymbol,IOutlineSymbol
    {
        PolygonSymbolType PolygonSymbolType { get; }
        RectangleF Bounds { get; set; }
        void DrawPolygon(Graphics context, float scale, GraphicsPath path);
        Brush GetBrush();
    }
}
