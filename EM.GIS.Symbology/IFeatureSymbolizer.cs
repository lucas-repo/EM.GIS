namespace EM.GIS.Symbology
{
    public interface IFeatureSymbolizer:ISymbolizer
    {
        ScaleMode ScaleMode { get; set; }
        double GetScale(MapArgs drawArgs);

        IFeatureSymbolCollection Symbols { get; set; }
    }
}
