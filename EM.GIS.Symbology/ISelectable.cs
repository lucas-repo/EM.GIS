using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可选择图层的接口
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// 是否可选择
        /// </summary>
        bool SelectionEnabled { get; set; }
        /// <summary>
        /// 选择事件已挂起
        /// </summary>
        bool SelectionChangesIsSuspended { get; }
        /// <summary>
        /// 清空选择
        /// </summary>
        /// <param name="affectedArea">影响范围</param>
        /// <param name="force">强制</param>
        /// <returns>成功与否</returns>
        bool ClearSelection(out IExtent affectedArea, bool force);
        /// <summary>
        /// 反向选择
        /// </summary>
        /// <param name="tolerant">宽松范围</param>
        /// <param name="strict">严格范围</param>
        /// <param name="mode">模式</param>
        /// <param name="affectedArea">影响范围</param>
        /// <returns>成功与否</returns>
        bool InvertSelection(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea);
        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="tolerant">宽松范围</param>
        /// <param name="strict">严格范围</param>
        /// <param name="mode">模式</param>
        /// <param name="affectedArea">影响范围</param>
        /// <param name="clear">清空状态</param>
        /// <returns>成功与否</returns>
        bool Select(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea, ClearStates clear);
        /// <summary>
        /// 清空选择
        /// </summary>
        /// <param name="tolerant">宽松范围</param>
        /// <param name="strict">严格范围</param>
        /// <param name="mode">模式</param>
        /// <param name="affectedArea">影响范围</param>
        /// <returns>成功与否</returns>
        bool UnSelect(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea);
    }
}
