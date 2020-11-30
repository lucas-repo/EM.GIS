using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface ILineSymbolCollection:IFeatureSymbolCollection
    {
        new ILineSymbol this[int index] { get; set; }
        new ILineSymbolizer Parent { get; set; }
        new IEnumerator<ILineSymbol> GetEnumerator();
    }
}