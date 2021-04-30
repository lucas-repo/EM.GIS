using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类集合
    /// </summary>
    public class LineCategoryCollection : FeatureCategoryCollection, ILineCategoryCollection
    {
        public new ILineCategory this[int index] { get => Items[index] as ILineCategory; set => Items[index] = value; }

        public LineCategoryCollection(IFeatureLayer parent) : base(parent)
        { }
    }
}