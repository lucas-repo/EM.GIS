using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    [Serializable]
    public class LineSymbolCollection : FeatureSymbolCollection, ILineSymbolCollection
    {
       public new ILineSymbol this[int index] { get => base[index] as ILineSymbol; set => base[index]=value; }

        public new ILineSymbolizer Parent { get => base.Parent as ILineSymbolizer; set => base.Parent = value; }
       
    }
}
