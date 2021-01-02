using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面分类集合
    /// </summary>
    public interface IPolygonCategoryCollection : IFeatureCategoryCollection, IEnumerable<IPolygonCategory>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new IPolygonCategory this[int index] { get; set; }
        #endregion
    }
}