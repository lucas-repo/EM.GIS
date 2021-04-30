using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    public class PolygonSymbolCollection : FeatureSymbolCollection, IPolygonSymbolCollection
    {
        public PolygonSymbolCollection(IPolygonSymbolizer parent) : base(parent)
        {
        }

        public new IPolygonSymbol this[int index] { get => base[index] as IPolygonSymbol; set => base[index] = value; }

        public new IPolygonSymbolizer Parent { get => base.Parent as IPolygonSymbolizer; set => base.Parent = value; }
    }
}
