using System;
using System.Collections.Generic;
using System.Linq;
namespace EM.GIS.Symbology
{
    [Serializable]
    public abstract class FeatureSymbolCollection : SymbolCollection, IFeatureSymbolCollection
    {
        public new IFeatureSymbol this[int index] { get => base[index] as IFeatureSymbol; set => base[index] = value; }

        public new IFeatureSymbolizer Parent { get => base.Parent as IFeatureSymbolizer; set => base.Parent = value; }

        public void Add(IFeatureSymbol item)
        {
            base.Add(item);
        }

        public bool Contains(IFeatureSymbol item)
        {
            return base.Contains(item);
        }

        public void CopyTo(IFeatureSymbol[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public int IndexOf(IFeatureSymbol item)
        {
            return base.IndexOf(item);
        }

        public void Insert(int index, IFeatureSymbol item)
        {
            base.Insert(index, item);
        }

        public bool Remove(IFeatureSymbol item)
        {
            return base.Remove(item);
        }


        public new IEnumerator<IFeatureSymbol> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as IFeatureSymbol;
            }
        }
    }
}