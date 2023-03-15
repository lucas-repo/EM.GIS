using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 选择器
    /// </summary>
    public abstract class Selection : Changeable, ISelection
    {
        /// <inheritdoc/>
        public abstract IExtent Extent { get; }
        /// <inheritdoc/>
        public ICategory? Category { get; set; }
        /// <inheritdoc/>
        public IProgressHandler? ProgressHandler { get; set; }
        /// <inheritdoc/>
        public SelectionMode SelectionMode { get; set; }
        /// <inheritdoc/>
        public bool SelectionState { get; set; }

        /// <inheritdoc/>
        public abstract bool AddRegion(IExtent region, out IExtent affectedArea);
        /// <inheritdoc/>
        public abstract bool InvertSelection(IExtent region, out IExtent affectedArea);
        /// <inheritdoc/>
        public abstract bool RemoveRegion(IExtent region, out IExtent affectedArea);
    }
}