using EM.Bases;
using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 分组
    /// </summary>
    public interface IGroup : IRenderableItem
    {
        /// <summary>
        /// 图层集合
        /// </summary>
        new IRenderableItemCollection Children { get; }
        /// <summary>
        /// 选择改变事件
        /// </summary>
        event EventHandler? SelectionChanged;
        /// <summary>
        /// 恢复选择事件
        /// </summary>
        void ResumeSelectionChanges();
        /// <summary>
        /// 暂停选择事件
        /// </summary>
        void SuspendSelectionChanges();
    }
}