using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface ILineSymbolizer:IFeatureSymbolizer
    {
        Color Color { get; set; }
        float Width { get; set; }
        new ILineSymbolCollection Symbols { get; set; }
        void DrawLine(Graphics context, float scale, GraphicsPath path);
    }
}
