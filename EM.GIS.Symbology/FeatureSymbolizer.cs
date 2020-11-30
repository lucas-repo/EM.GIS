using EM.GIS.Data;



namespace EM.GIS.Symbology
{
    public abstract class FeatureSymbolizer : Symbolizer, IFeatureSymbolizer
    {
        public ScaleMode ScaleMode { get; set; } = ScaleMode.Symbolic;

        public IFeatureSymbolCollection Symbols { get;  set; }

        public double GetScale(MapArgs drawArgs)
        {
            if (ScaleMode == ScaleMode.Geographic)
            {
                return drawArgs.Bounds.Width / drawArgs.Extent.Width;
            }
            return 1;
        }
    }
}