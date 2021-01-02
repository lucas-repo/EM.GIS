using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线符号集合
    /// </summary>
    public interface ILineSymbolCollection:IFeatureSymbolCollection
    {
        new ILineSymbol this[int index] { get; set; }
        new ILineSymbolizer Parent { get; set; }
    }
}