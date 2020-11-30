using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface IPolygonSymbolCollection:IFeatureSymbolCollection
    {
        new IPolygonSymbol this[int index] { get; set; }
        new IPolygonSymbolizer Parent { get; set; }
        new IEnumerator<IPolygonSymbol> GetEnumerator();
    }
}