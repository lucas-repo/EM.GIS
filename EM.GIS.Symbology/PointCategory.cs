using System.Drawing;



namespace EM.GIS.Symbology
{
    public class PointCategory : FeatureCategory, IPointCategory
    {
        public new IPointSymbolizer SelectionSymbolizer { get=>base.SelectionSymbolizer as IPointSymbolizer; set => base.SelectionSymbolizer=value; }
        public new IPointSymbolizer Symbolizer { get => base.Symbolizer as IPointSymbolizer; set => base.Symbolizer = value; }
        public PointCategory()
        {
            Symbolizer = new PointSymbolizer();
            SelectionSymbolizer = new PointSymbolizer(true);
        }
        public PointCategory(IPointSymbolizer pointSymbolizer)
        {
            Symbolizer = pointSymbolizer;
            SelectionSymbolizer = pointSymbolizer.Clone() as IPointSymbolizer;
            SelectionSymbolizer.Symbols[0].Color = Color.Cyan;
        }

        public override void DrawLegend(Graphics context, Rectangle rectangle)
        {
            Symbolizer?.DrawLegend(context, rectangle);
        }
    }
}
