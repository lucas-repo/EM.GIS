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
    public interface ILayer : ILegendItem, IDisposable,IDynamicVisibility
    {
        /// <summary>
        /// 数据集
        /// </summary>
        IDataSet DataSet { get; set; }
        /// <summary>
        /// 进度处理
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get;  }

        /// <summary>
        /// 分类集合（要素或栅格等子图层才实现）
        /// </summary>
        ICategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        ICategory DefaultCategory { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        ISelection Selection { get; }
        /// <summary>
        /// 在指定范围是否可见
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        bool GetVisible(IExtent extent,Rectangle rectangle);
        /// <summary>
        /// 绘制图层到画布
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="rectangle"></param>
        /// <param name="extent"></param>
        /// <param name="selected"></param>
        /// <param name="cancellationTokenSource"></param>
        void Draw(Graphics graphics,Rectangle rectangle, IExtent extent, bool selected=false,  CancellationTokenSource cancellationTokenSource = null);
    }
}
