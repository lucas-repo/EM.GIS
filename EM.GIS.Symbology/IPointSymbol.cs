using System.Drawing;

namespace EM.GIS.Symbology
{
    public interface IPointSymbol: IFeatureSymbol,IOutlineSymbol
    {
        float Angle { get; set; }
        PointF Offset { get; set; }
        PointSymbolType PointSymbolType { get; }
        void DrawPoint(Graphics context, float scale, PointF point);
        SizeF Size { get; set; }
    }
}
