﻿using EM.GIS.Data;
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
    public interface IMap : IProj, INotifyPropertyChanged
    {
        /// <summary>
        /// 视图范围
        /// </summary>
        IExtent ViewExtent { get; set; }
        /// <summary>
        /// 视图边界
        /// </summary>
        Rectangle ViewBound { get; set; }
        /// <summary>
        /// 是否在工作中
        /// </summary>
        bool IsBusy { get; set; }
        /// <summary>
        /// 图例
        /// </summary>
        ILegend Legend { get; set; }
        /// <summary>
        /// 进度管理
        /// </summary>
        ProgressDelegate Progress { get; set; }
        /// <summary>
        /// 重绘指定区域
        /// </summary>
        /// <param name="rectangle"></param>
        void Invalidate(RectangleF rectangle);
        /// <summary>
        /// 重绘整个地图
        /// </summary>
        void Invalidate();
        /// <summary>
        /// 缩放至最大范围
        /// </summary>
        void ZoomToMaxExtent();
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame MapFrame { get; set; }
        /// <summary>
        /// 添加多个图层
        /// </summary>
        /// <returns></returns>
        IList<ILayer> AddLayers();
        /// <summary>
        /// 添加单个图层
        /// </summary>
        /// <returns></returns>
        ILayer AddLayer();
        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="groupName">分组名</param>
        /// <returns>分组</returns>
        IGroup AddGroup(string groupName = null);
        /// <summary>
        /// 图层
        /// </summary>
        ILayerCollection Layers { get; }
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

        #region 事件
        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        event EventHandler<IGeoMouseEventArgs> GeoMouseMove;
        #endregion
    }
}
