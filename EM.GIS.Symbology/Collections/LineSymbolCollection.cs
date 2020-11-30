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
       
        public void Add(ILineSymbol item)
        {
            base.Add(item);
        }

        public bool Contains(ILineSymbol item)
        {
            return base.Contains(item);
        }

        public void CopyTo(ILineSymbol[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public int IndexOf(ILineSymbol item)
        {
            return base.IndexOf(item);
        }

        public void Insert(int index, ILineSymbol item)
        {
            base.Insert(index, item);
        }

        public bool Remove(ILineSymbol item)
        {
            return base.Remove(item);
        }

        public new IEnumerator<ILineSymbol> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as ILineSymbol;
            }
        }
    }
}
