using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分类集合接口
    /// </summary>
    public interface ICategoryCollection : ILegendItemCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new ICategory this[int index] { get; set; }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        new IEnumerator<ICategory> GetEnumerator();
        /// <summary>
        /// 父元素
        /// </summary>
        new ILayer Parent { get; set; }
        #endregion
    }
}