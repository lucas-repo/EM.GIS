using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface IPolygonSymbolizer:IFeatureSymbolizer
    {
        new IPolygonSymbolCollection Symbols { get; set; }
        void DrawPolygon(Graphics context, float scale, GraphicsPath path);
    }
}
