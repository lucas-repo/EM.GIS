using System.Drawing;

namespace EM.GIS.Symbology
{
    public interface IFeatureSymbol: ISymbol
    {
        Color Color { get; set; }
        float Opacity { get; set; }
    }
}
