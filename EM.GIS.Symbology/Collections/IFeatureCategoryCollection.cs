using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素分类集合
    /// </summary>
    public interface IFeatureCategoryCollection : ICategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new IFeatureCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new IFeatureLayer Parent { get; set; }
        #endregion
    }
}