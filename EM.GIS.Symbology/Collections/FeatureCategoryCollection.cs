using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素分类集合
    /// </summary>
    public abstract class FeatureCategoryCollection : CategoryCollection, IFeatureCategoryCollection
    {
        public new IFeatureCategory this[int index] { get =>Items[index] as IFeatureCategory; set => Items[index] = value; }
        public new IFeatureLayer Parent { get => base.Parent as IFeatureLayer; set => base.Parent = value; }

        public FeatureCategoryCollection(IFeatureLayer parent) : base(parent)
        { }

    }
}