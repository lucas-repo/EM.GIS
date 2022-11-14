using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 标注分类集合
    /// </summary>
    public interface ILabelCategoryCollection:ICategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new ILabelCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new ILabelLayer Parent { get; set; }
        #endregion
    }
}