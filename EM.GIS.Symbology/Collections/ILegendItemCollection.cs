using EM.Bases;
using EM.GIS.Data;
using System.Collections;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素集合
    /// </summary>
    public interface ILegendItemCollection : IItemCollection<IBaseItem>,IParent<ILegendItem>
    {
        #region 需要重写的部分
        /// <summary>
        /// 获取或设置图层
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new ILegendItem this[int index] { get; set; }
        #endregion

        /// <summary>
        /// 进度
        /// </summary>
        ProgressDelegate Progress { get; set; }
    }
}