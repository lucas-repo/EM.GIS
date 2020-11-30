using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    public interface ILineSimpleSymbol:ILineSymbol
    {
        DashStyle DashStyle { get; set; }
    }
}
