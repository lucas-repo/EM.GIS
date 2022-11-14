using EM.Bases;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分类集合接口
    /// </summary>
    public interface ICategoryCollection : IItemCollection<IBaseItem>, IParent<ILayer>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置分类
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>分类</returns>
        new ICategory this[int index] { get; set; }
        #endregion
    }
}