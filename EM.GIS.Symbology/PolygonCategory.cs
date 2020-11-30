using System.Drawing;

namespace EM.GIS.Symbology
{
    public class PolygonCategory : FeatureCategory, IPolygonCategory
    {
        public new IPolygonSymbolizer SelectionSymbolizer { get => base.SelectionSymbolizer as IPolygonSymbolizer; set => base.SelectionSymbolizer = value; }
        public new IPolygonSymbolizer Symbolizer { get => base.Symbolizer as IPolygonSymbolizer; set => base.Symbolizer = value; }
        public PolygonCategory()
        {
            Symbolizer = new PolygonSymbolizer();
            SelectionSymbolizer = new PolygonSymbolizer(true);
        }

        public override void DrawLegend(Graphics context, Rectangle rectangle)
        {
            Symbolizer?.DrawLegend(context, rectangle);
        }
    }
}
