using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点分类集合
    /// </summary>
    public interface IPointCategoryCollection: IFeatureCategoryCollection
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new IPointCategory this[int index] { get; set; }
        /// <summary>
        /// 父图层
        /// </summary>
        new IPointLayer Parent { get; set; }

        #endregion
    }
}
