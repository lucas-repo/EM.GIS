using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类集合
    /// </summary>
    public interface ILineCategoryCollection : IFeatureCategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new ILineCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new ILineLayer Parent { get; set; }
        #endregion
    }
}