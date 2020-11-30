using EM.GIS.Geometries;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架接口
    /// </summary>
    public interface IFrame : IGroup
    {
        /// <summary>
        /// 地图框是否忙于绘制
        /// </summary>
        bool IsBusy { get; set; }
        /// <summary>
        /// 临时绘制图层
        /// </summary>
        ILayerCollection DrawingLayers { get; }
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
        /// 视图边界
        /// </summary>
        Rectangle ViewBounds { get; set; }
        /// <summary>
        /// 视图范围
        /// </summary>
        IExtent ViewExtents { get; set; }
        /// <summary>
        /// 取消标记源
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; set; }
        /// <summary>
        /// 缓存图片改变事件
        /// </summary>
        event EventHandler BufferChanged;
        /// <summary>
        /// 重绘缓存
        /// </summary>
        /// <param name="extent"></param>
        Task ResetBuffer(IExtent extent = null);
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="g"></param>
        /// <param name="rectangle"></param>
        void Draw(Graphics g, Rectangle rectangle);

        /// <summary>
        /// 根据视图范围重设范围
        /// </summary>
        void ResetExtents();
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