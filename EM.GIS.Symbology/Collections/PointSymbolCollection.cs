using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    public class PointSymbolCollection :FeatureSymbolCollection, IPointSymbolCollection
    {
        public new IPointSymbol this[int index] { get => base[index] as IPointSymbol; set => base[index] = value; }

        public new IPointSymbolizer Parent { get => base.Parent as IPointSymbolizer; set => base.Parent = value; }

        public new IEnumerator<IPointSymbol> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as IPointSymbol;
            }
        }
    }
}