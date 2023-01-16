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
    }
}