using EM.GIS.Geometries;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架接口
    /// </summary>
    public interface IFrame : IGroup
    {
        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get;}
        /// <summary>
        /// 是否已更改
        /// </summary>
        bool IsDirty { get; }
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
        /// <summary>
        /// 清空数据和状态
        /// </summary>
        void New();
        /// <summary>
        /// 打开地图
        /// </summary>
        /// <param name="fileName">地图文件</param>
        void Open(string fileName);
        /// <summary>
        /// 保存
        /// </summary>
        void Save();
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName">地图文件</param>
        void SaveAs(string fileName);
    }
}