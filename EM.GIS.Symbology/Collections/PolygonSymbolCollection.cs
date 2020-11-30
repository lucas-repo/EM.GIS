using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    public class PolygonSymbolCollection:FeatureSymbolCollection, IPolygonSymbolCollection
    {
        public new IPolygonSymbol this[int index] { get => base[index] as IPolygonSymbol; set => base[index] = value; }

        public new IPolygonSymbolizer Parent { get => base.Parent as IPolygonSymbolizer; set => base.Parent = value; }
        public new IEnumerator<IPolygonSymbol> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as IPolygonSymbol;
            }
        }
    }
}
