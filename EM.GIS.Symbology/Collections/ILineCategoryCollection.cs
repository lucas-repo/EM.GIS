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
        /// <param name="index"></param>
        /// <returns></returns>
        new ILineCategory this[int index] { get; set; }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        new IEnumerator<ILineCategory> GetEnumerator();
        #endregion
    }
}