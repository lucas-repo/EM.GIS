using EM.Bases;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点分类集合
    /// </summary>
    public class PointCategoryCollection : FeatureCategoryCollection,  IPointCategoryCollection
    {
        public new IPointCategory this[int index] { get => Items[index] as IPointCategory; set => Items[index] = value; }
        public new IPointLayer Parent { get => base.Parent as IPointLayer; set => base.Parent = value; }

        public PointCategoryCollection(IPointLayer parent) : base(parent)
        { }
        public override void Add(IBaseItem item)
        {
            if (item is IPointCategory)
            {
                base.Add(item);
            }
        }
    }
}