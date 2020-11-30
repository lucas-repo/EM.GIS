using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface ILineSymbol : IFeatureSymbol
    {
        float Width { get; set; }
        LineSymbolType LineSymbolType { get; }
        void DrawLine(Graphics context, float scale, GraphicsPath path);
        Pen ToPen(float scale);
    }
}
