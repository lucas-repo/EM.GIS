using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面符号集合
    /// </summary>
    public interface IPolygonSymbolCollection:IFeatureSymbolCollection
    {
        new IPolygonSymbol this[int index] { get; set; }
        new IPolygonSymbolizer Parent { get; set; }
    }
}