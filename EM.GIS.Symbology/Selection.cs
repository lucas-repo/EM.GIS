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
        public virtual IExtent IExtent { get; set; }
        public ICategory Category { get; set; }
        public IProgressHandler ProgressHandler { get; set; }
    }
}