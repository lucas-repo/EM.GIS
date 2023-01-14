using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 包含图层操作的框架接口
    /// </summary>
    public interface IFrame : IGroup
    {
        /// <summary>
        /// 视图
        /// </summary>
        IView View { get; }
        /// <summary>
        /// 投影
        /// </summary>
        IProjection Projection { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get;}
        /// <summary>
        /// 是否已更改
        /// </summary>
        bool IsDirty { get; }
        /// <summary>
        /// 第一个图层添加事件
        /// </summary>
        event EventHandler? FirstLayerAdded;
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