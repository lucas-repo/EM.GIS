using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层接口
    /// </summary>
    public interface ILayer : ILegendItem,IDynamicVisibility, IDrawable
    {
        /// <summary>
        /// 数据集
        /// </summary>
        IDataSet DataSet { get; set; }
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; set; }
        /// <summary>
        /// 父图层组
        /// </summary>
        new IGroup Parent { get; set; }
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame Frame { get; set; }
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        bool GetVisible(IExtent extent,Rectangle rectangle);
    }
}
