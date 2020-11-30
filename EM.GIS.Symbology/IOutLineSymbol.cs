using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface IOutlineSymbol:IFeatureSymbol
    {
        bool UseOutLine { get; set; }
        ILineSymbolizer OutLineSymbolizer { get; set; }
        void CopyOutLine(IOutlineSymbol outlineSymbol);
        void DrawOutLine(Graphics context, float scale,GraphicsPath path);
    }
}