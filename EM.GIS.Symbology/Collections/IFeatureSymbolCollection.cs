using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public interface IFeatureSymbolCollection: ISymbolCollection, IEnumerable<IFeatureSymbol>    
    {
        new IFeatureSymbol this[int index] { get;set; }
        new IFeatureSymbolizer Parent { get; set; }
    }
}