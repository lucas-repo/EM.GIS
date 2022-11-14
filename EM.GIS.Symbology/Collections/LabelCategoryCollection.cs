using EM.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 标注分类集合
    /// </summary>
    public class LabelCategoryCollection: CategoryCollection,ILabelCategoryCollection
    {
        public new ILabelCategory this[int index] { get => Items[index] as ILabelCategory; set => Items[index] = value; }
        public new ILabelLayer Parent { get => base.Parent as ILabelLayer; set => base.Parent = value; }

        public LabelCategoryCollection(ILabelLayer parent) : base(parent)
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
