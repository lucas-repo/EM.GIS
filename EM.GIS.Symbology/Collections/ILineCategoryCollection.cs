using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类集合
    /// </summary>
    public interface ILineCategoryCollection : IFeatureCategoryCollection, IEnumerable<ILineCategory>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new ILineCategory this[int index] { get; set; }
        #endregion
    }
}