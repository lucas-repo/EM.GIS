using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格分类集合
    /// </summary>
    public interface IRasterCategoryCollection: ICategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new IRasterCategory this[int index] { get; set; }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns></returns>
        new IEnumerator<IRasterCategory> GetEnumerator();
        /// <summary>
        /// 父图层
        /// </summary>
        new IRasterLayer Parent { get; set; }
        #endregion
    }
}
