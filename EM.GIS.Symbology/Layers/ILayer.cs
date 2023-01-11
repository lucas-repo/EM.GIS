using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层接口
    /// </summary>
    public interface ILayer : ILegendItem,IDynamicVisibility, IDrawableLayer
    {
        /// <summary>
        /// 父图层组
        /// </summary>
        new IGroup Parent { get; set; }
        /// <summary>
        /// 分类集合
        /// </summary>
        new ICategoryCollection Children { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        ICategory DefaultCategory { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        ISelection Selection { get; }

        /// <summary>
        /// 数据集
        /// </summary>
        IDataSet DataSet { get; set; }
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; set; }
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame Frame { get; set; }
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent">范围</param>
        /// <param name="rectangle">矩形</param>
        /// <returns>可见为true反之false</returns>
        bool GetVisible(IExtent extent,Rectangle rectangle);
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent">范围</param>
        /// <returns>可见为true反之false</returns>
        bool GetVisible(IExtent extent);
    }
}
