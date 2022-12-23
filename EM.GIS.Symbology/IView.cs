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
    public interface IView : IProj, ICloneable, INotifyPropertyChanged, ICancelable, IDisposable
    {
        /// <summary>
        /// 地图框架
        /// </summary>
        IFrame Frame { get; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        Color BackGround { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 高度
        /// </summary>
        int Height { get; }
        /// <summary>
        /// 后台缓存图片，用以获取缓存图片
        /// </summary>
        Image BackBuffer { get; set; }
        /// <summary>
        /// 视图范围
        /// </summary>
        new IExtent Extent { get; set; }
        /// <summary>
        /// 视图边界
        /// </summary>
        Rectangle ViewBound { get; set; }
        /// <summary>
        /// 进度委托
        /// </summary>
        Action<string, int> Progress { get; set; }
        /// <summary>
        /// 重绘缓存
        /// </summary>
        void ResetBuffer();
        /// <summary>
        /// 绘制背景图至指定画板的指定范围
        /// </summary>
        /// <param name="g">画板</param>
        /// <param name="rectangle">范围</param>
        void Draw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 根据视图边界重设视图范围
        /// </summary>
        void ResetViewExtent();
        /// <summary>
        /// 重设地图大小
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void Resize(int width, int height);
        /// <summary>
        /// 居中至最大范围
        /// </summary>
        void ZoomToMaxExtent();
    }
}