using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点分类集合
    /// </summary>
    public class PointCategoryCollection : FeatureCategoryCollection,  IPointCategoryCollection
    {
        public new IPointCategory this[int index] { get => Items[index] as IPointCategory; set => Items[index] = value; }

        public PointCategoryCollection()
        { }
        public PointCategoryCollection(IFeatureLayer parent) : base(parent)
        { }
    }
}