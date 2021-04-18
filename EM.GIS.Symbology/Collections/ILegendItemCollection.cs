using EM.GIS.Data;
using System.Collections;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素集合
    /// </summary>
    public interface ILegendItemCollection : IItemCollection<ILegendItem, ILegendItem>
    {
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
    }
}