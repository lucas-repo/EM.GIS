namespace EM.GIS.Symbology
{
    public abstract class SymbolCollection:LegendItemCollection,ISymbolCollection
    {
        public SymbolCollection(ISymbolizer parent):base(parent)
        {
        }

        public new ISymbol this[int index] { get => Items[index] as IFeatureSymbol; set => Items[index] = value; }

        public new ISymbolizer Parent { get => base.Parent as IFeatureSymbolizer; set => base.Parent = value; }
    }
}