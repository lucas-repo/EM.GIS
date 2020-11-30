namespace EM.GIS.Symbology
{
    public abstract class FeatureCategory : Category, IFeatureCategory
    {
        public string FilterExpression { get; set; }
        private IFeatureSymbolizer _selectionSymbolizer;
        public IFeatureSymbolizer SelectionSymbolizer
        {
            get => _selectionSymbolizer;
            set
            {
                value.Parent = this;
                _selectionSymbolizer = value;
            } 
        }
        private IFeatureSymbolizer _symbolizer;
        public IFeatureSymbolizer Symbolizer
        {
            get => _symbolizer;
            set
            {
                value.Parent = this;
                _symbolizer = value;
            }
        }
    }
}