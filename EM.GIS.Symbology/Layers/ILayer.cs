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
        /// 范围
        /// </summary>
        IExtent Extent { get; set; }
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
        /// 父图层组
        /// </summary>
        new IGroup Parent { get; set; }
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
        /// <param name="graphics">画布</param>
        /// <param name="rectangle">屏幕范围</param>
        /// <param name="extent">世界范围</param>
        /// <param name="selected">是否选择</param>
        /// <param name="cancelFunc">取消绘制委托</param>
        /// <param name="invalidateMapFrameAction">使地图框无效委托（重绘）</param>
        void Draw(Graphics graphics,Rectangle rectangle, IExtent extent, bool selected=false, Func<bool> cancelFunc = null, Action invalidateMapFrameAction=null);
    }
}
