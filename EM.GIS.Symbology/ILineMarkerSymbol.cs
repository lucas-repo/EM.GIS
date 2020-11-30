namespace EM.GIS.Symbology
{
    public interface ILineMarkerSymbol : ILineCartographicSymbol
    {
        IPointSymbolizer Marker { get; set; }
    }
}