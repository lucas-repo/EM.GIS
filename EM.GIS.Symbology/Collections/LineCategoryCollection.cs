using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类集合
    /// </summary>
    public class LineCategoryCollection : FeatureCategoryCollection, ILineCategoryCollection
    {
        public new ILineCategory this[int index] { get => Items[index] as ILineCategory; set => Items[index] = value; }

        public LineCategoryCollection()
        { }
        public LineCategoryCollection(IFeatureLayer parent) : base(parent)
        { }
        public new IEnumerator<ILineCategory> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item as ILineCategory;
            }
        }
    }
}