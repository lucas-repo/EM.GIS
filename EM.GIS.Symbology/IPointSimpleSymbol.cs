namespace EM.GIS.Symbology
{
    public interface IPointSimpleSymbol : IPointSymbol
    {
        PointShape PointShape { get; set; }
    }
}