using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 选择接口
    /// </summary>
    public interface ISelection:IChangeable
    {
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; }
        /// <summary>
        /// 分类
        /// </summary>
        ICategory? Category { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler? ProgressHandler { get; set; }
        /// <summary>
        /// 选择模式
        /// </summary>
        SelectionMode SelectionMode { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        bool SelectionState { get; set; }
        /// <summary>
        /// 添加范围选择
        /// </summary>
        /// <param name="region">范围</param>
        /// <param name="affectedArea">影响范围</param>
        /// <returns>成功与否</returns>
        bool AddRegion(IExtent region, out IExtent affectedArea);
        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="region">范围</param>
        /// <param name="affectedArea">影响范围</param>
        /// <returns>成功与否</returns>
        bool InvertSelection(IExtent region, out IExtent affectedArea);
        /// <summary>
        /// 移除指定范围的选择
        /// </summary>
        /// <param name="region">范围</param>
        /// <param name="affectedArea">影响范围</param>
        /// <returns>成功与否</returns>
        bool RemoveRegion(IExtent region, out IExtent affectedArea);

    }
}