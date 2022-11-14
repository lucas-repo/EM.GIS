using EM.Bases;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面分类集合
    /// </summary>
    public class PolygonCategoryCollection :FeatureCategoryCollection, IPolygonCategoryCollection
    {
        public new IPolygonCategory this[int index] { get => Items[index] as IPolygonCategory; set => Items[index] = value; }
        public new IPolygonLayer Parent { get => base.Parent as IPolygonLayer; set => base.Parent = value; }

        public PolygonCategoryCollection(IPolygonLayer parent) : base(parent)
        { }
        public override void Add(IBaseItem item)
        {
            if (item is IPolygonCategory)
            {
                base.Add(item);
            }
        }
    }
}