using EM.GIS.Data;
using EM.GIS.Geometries;
using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 地图接口
    /// </summary>
    public interface IMap :  INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame Frame { get; set; }
        /// <summary>
        /// 是否在工作中
        /// </summary>
        bool IsBusy { get; set; }
        /// <summary>
        /// 图例
        /// </summary>
        ILegend Legend { get; set; }
        /// <summary>
        /// 地图方法
        /// </summary>
        List<ITool> MapTools { get; }
        /// <summary>
        /// 激活工具
        /// </summary>
        /// <param name="tool">工具</param>  
        void ActivateMapToolWithZoom(ITool tool);
        /// <summary>
        /// 使所有地图工具无效
        /// </summary>
        void DeactivateAllMapTools();
        /// <summary>
        /// 使控件指定区域无效，在UI线程可用时进行重绘
        /// </summary>
        /// <param name="rectangle">屏幕范围</param>
        void Invalidate(RectangleF rectangle);
        #region 事件
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        event EventHandler<IGeoMouseEventArgs> GeoMouseMove;
        #endregion
    }
}
