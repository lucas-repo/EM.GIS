using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点符号集合
    /// </summary>
    public interface IPointSymbolCollection:IFeatureSymbolCollection 
    {
        new IPointSymbol this[int index] { get; set; }
        new IPointSymbolizer Parent { get; set; }
    }
}