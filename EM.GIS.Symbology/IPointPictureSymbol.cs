using System.Drawing;

namespace EM.GIS.Symbology
{
    public interface IPointPictureSymbol:IPointSymbol
    {
        Image Image { get; set; }
        string ImageBase64 { get; set; }
        string ImagePath { get; set; }
    }
}