using EM.GIS.Geometries;
using System.Drawing;


namespace EM.GIS.Symbology
{
    public interface IGetImage
    {
        Bitmap GetImage(IExtent envelope, Rectangle rectangle) ;
    }
}