using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public abstract class OutLineSymbol : FeatureSymbol, IOutlineSymbol
    {
        public bool UseOutLine { get; set; } = true;
        public ILineSymbolizer OutLineSymbolizer { get; set; }
        public OutLineSymbol()
        {
            OutLineSymbolizer = new LineSymbolizer();
        }

        public void CopyOutLine(IOutlineSymbol outlineSymbol)
        {
            UseOutLine = outlineSymbol.UseOutLine;
            OutLineSymbolizer = outlineSymbol.OutLineSymbolizer.Clone() as ILineSymbolizer;
        }

        public void DrawOutLine(Graphics context, float scale, GraphicsPath path)
        {
            if (UseOutLine && OutLineSymbolizer != null)
            {
                OutLineSymbolizer.DrawLine(context, scale, path);
            }
        }
    }
}