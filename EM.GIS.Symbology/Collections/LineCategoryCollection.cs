using EM.Bases;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类集合
    /// </summary>
    public class LineCategoryCollection : FeatureCategoryCollection, ILineCategoryCollection
    {
        public new ILineCategory this[int index] { get => Items[index] as ILineCategory; set => Items[index] = value; }
        public new ILineLayer Parent { get => base.Parent as ILineLayer; set => base.Parent = value; }

        public LineCategoryCollection(ILineLayer parent) : base(parent)
        { }
        public override void Add(IBaseItem item)
        {
            if (item is ILineCategory)
            {
                base.Add(item);
            }
        }
    }
}