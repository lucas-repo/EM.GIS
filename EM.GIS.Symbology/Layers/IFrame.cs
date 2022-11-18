using EM.GIS.Geometries;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架接口
    /// </summary>
    public interface IFrame : IGroup
    {
        /// <summary>
        /// 地图视图
        /// </summary>
        IView MapView { get; }
        /// <summary>
        /// 临时绘制图层
        /// </summary>
        ILayerCollection DrawingLayers { get; }
        /// <summary>
        /// 计算最大范围
        /// </summary>
        /// <param name="expand">是否扩展</param>
        /// <returns>最大范围</returns>
        IExtent GetMaxExtent(bool expand = false);
    }
}