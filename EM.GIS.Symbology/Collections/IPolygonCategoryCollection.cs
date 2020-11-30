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
        /// <param name="index"></param>
        /// <returns></returns>
        new IPolygonCategory this[int index] { get; set; }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        new IEnumerator<IPolygonCategory> GetEnumerator();
        #endregion
    }
}