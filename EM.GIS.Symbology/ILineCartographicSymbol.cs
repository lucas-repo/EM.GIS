using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface ILineCartographicSymbol:ILineSimpleSymbol
    {
        float[] DashPattern { get; set; }
        List<ILineDecoration> Decorations { get; }
    }
}