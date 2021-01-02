using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 标注分类集合
    /// </summary>
    public interface ILabelCategoryCollection:ICategoryCollection
    {
        new ILabelCategory this[int index] { get; set; }
        new ILabelLayer Parent { get; set; }
    }
}