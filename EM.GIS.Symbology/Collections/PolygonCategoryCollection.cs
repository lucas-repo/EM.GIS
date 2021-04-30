using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面分类集合
    /// </summary>
    public class PolygonCategoryCollection :FeatureCategoryCollection, IPolygonCategoryCollection
    {
        public new IPolygonCategory this[int index] { get => Items[index] as IPolygonCategory; set => Items[index] = value; }

        public PolygonCategoryCollection(IFeatureLayer parent) : base(parent)
        { }
    }
}