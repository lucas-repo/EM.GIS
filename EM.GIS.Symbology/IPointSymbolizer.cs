using System.Drawing;

namespace EM.GIS.Symbology
{
    public interface IPointSymbolizer:IFeatureSymbolizer
    {
        new IPointSymbolCollection Symbols { get; set; }
        SizeF Size { get; set; }
        void DrawPoint(Graphics context, float scale, PointF point);
    }
}
