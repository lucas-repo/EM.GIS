using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.ComponentModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 视图接口
    /// </summary>
    public interface IView :  ICloneable, INotifyPropertyChanged,  IDisposable,IProj
    {
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame Frame { get; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        Color Background { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 高度
        /// </summary>
        int Height { get; }
        /// <summary>
        /// 正在工作
        /// </summary>
        bool IsWorking { get; }
        /// <summary>
        /// 后台缓存图片
        /// </summary>
        ViewCache BackImage { get; }
        /// <summary>
        /// 视图窗口
        /// </summary>
        RectangleF ViewBound { get; set; }
        /// <summary>
        /// 视图范围
        /// </summary>
        IExtent ViewExtent { get; set; }
        /// <summary>
        /// 比例尺因子
        /// </summary>
         double ScaleFactor { get; set; }
        /// <summary>
        /// 进度委托
        /// </summary>
        Action<string, int>? Progress { get; set; }
        /// <summary>
        /// 重绘缓存
        /// </summary>
        /// <param name="rectangle">画布大小</param>
        /// <param name="extent">地图范围</param>
        /// <param name="drawingExtent">重绘范围</param>
        void ResetBuffer(Rectangle rectangle, IExtent extent, IExtent drawingExtent);
        /// <summary>
        /// 绘制背景图至指定画板的指定范围
        /// </summary>
        /// <param name="g">画板</param>
        /// <param name="rectangle">范围</param>
        void Draw(Graphics g, RectangleF rectangle);
        /// <summary>
        /// 根据视图矩形重设视图范围
        /// </summary>
        void ResetViewExtent();
        /// <summary>
        /// 重设地图大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void Resize(int width, int height);
        /// <summary>
        /// 缩放至最大范围
        /// </summary>
        void ZoomToMaxExtent();
    }
}