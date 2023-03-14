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
        event EventHandler SelectionChanged;
        bool SelectionEnabled { get; set; }
        bool SelectionChangesIsSuspended { get; }
        bool ClearSelection(out IExtent affectedArea, bool force);
        bool InvertSelection(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea);
        bool Select(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea, ClearStates clear);
        bool UnSelect(IExtent tolerant, IExtent strict, SelectionMode mode, out IExtent affectedArea);
        void ResumeSelectionChanges();
        void SuspendSelectionChanges();
    }
}
