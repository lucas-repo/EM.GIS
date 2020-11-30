using System.Drawing;
using System.Globalization;

namespace EM.GIS.Symbology
{
    public interface IPointCharacterSymbol:IPointSymbol
    {
        UnicodeCategory Category { get; }

        char Character { get; set; }

        string FontFamilyName { get; set; }

        FontStyle FontStyle { get; set; }

        string ToString();
    }
}