using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可渲染元素接口
    /// </summary>
    public interface IRenderableItem: ILegendItem, IDynamicVisibility
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
        /// <summary>
        /// 是否已初始化绘制
        /// </summary>
        /// <param name="proj">画布大小及范围</param>
        /// <param name="extent">要初始化的范围</param>
        /// <returns>已初始化为true反之false</returns>
        bool IsDrawingInitialized(IProj proj, IExtent extent);
        /// <summary>
        /// 初始化绘制
        /// </summary>
        /// <param name="proj">画布大小及范围</param>
        /// <param name="extent">要初始化的范围</param>
        /// <param name="cancelFunc">取消委托</param>
        void InitializeDrawing(IProj proj, IExtent extent, Func<bool>? cancelFunc = null);
        /// <summary>
        /// 绘制到画布
        /// </summary>
        /// <param name="mapArgs">绘制参数</param>
        /// <param name="onlyInitialized">是否只绘制已初始化的</param>
        /// <param name="selected">是否选择</param>
        /// <param name="progressAction">进度委托</param>
        /// <param name="cancelFunc">取消委托</param>
        /// <param name="invalidateMapFrameAction">重绘地图委托</param>
        /// <returns>返回绘制的区域，未绘制则返回空矩形</returns>
        Rectangle Draw(MapArgs mapArgs,bool onlyInitialized=false, bool selected = false, Action<string, int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? invalidateMapFrameAction = null);
    }
}
