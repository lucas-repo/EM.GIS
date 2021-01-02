using System;
using System.Collections.Generic;
using System.Linq;
namespace EM.GIS.Symbology
{
    [Serializable]
    public abstract class FeatureSymbolCollection : SymbolCollection, IFeatureSymbolCollection
    {
        public new IFeatureSymbol this[int index] { get => Items[index] as IFeatureSymbol; set => Items[index] = value; }

        public new IFeatureSymbolizer Parent { get => base.Parent as IFeatureSymbolizer; set => base.Parent = value; }

        public IEnumerator<IFeatureSymbol> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as IFeatureSymbol;
            }
        }
    }
}