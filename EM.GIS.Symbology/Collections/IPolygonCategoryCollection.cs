using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面分类集合
    /// </summary>
    public interface IPolygonCategoryCollection : IFeatureCategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new IPolygonCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new IPolygonLayer Parent { get; set; }
        #endregion
    }
}