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
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new IRasterCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new IRasterLayer Parent { get; set; }
        #endregion
    }
}
