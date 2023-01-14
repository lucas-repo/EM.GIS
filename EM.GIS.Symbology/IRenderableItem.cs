using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可渲染元素接口
    /// </summary>
    public interface IRenderableItem: ILegendItem, IDynamicVisibility, IDrawableLayer
    {
        /// <summary>
        /// 父图层组
        /// </summary>
        new IGroup? Parent { get; set; }
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; }
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent">范围</param>
        /// <param name="rectangle">矩形</param>
        /// <returns>可见为true反之false</returns>
        bool GetVisible(IExtent extent, Rectangle rectangle);
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent">范围</param>
        /// <returns>可见为true反之false</returns>
        bool GetVisible(IExtent extent);
    }
}
