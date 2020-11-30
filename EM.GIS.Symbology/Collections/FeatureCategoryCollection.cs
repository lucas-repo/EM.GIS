using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素分类集合
    /// </summary>
    public abstract class FeatureCategoryCollection : CategoryCollection, IFeatureCategoryCollection
    {
        public new IFeatureLayer Parent { get => base.Parent as IFeatureLayer; set => base.Parent = value; }

        public new IFeatureCategory this[int index] { get => base[index] as IFeatureCategory; set => base[index] = value; }

        public FeatureCategoryCollection()
        { }
        public FeatureCategoryCollection(IFeatureLayer parent) : base(parent)
        { }

        public new IEnumerator<IFeatureCategory> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as IFeatureCategory;
            }
        }
    }
}