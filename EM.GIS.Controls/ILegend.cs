using EM.GIS.Symbology;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 图例接口
    /// </summary>
    public interface ILegend
    {
        /// <summary>
        /// 元素集合
        /// </summary>
        ILegendItemCollection LegendItems { get; }
        /// <summary>
        /// 添加地图框架
        /// </summary>
        /// <param name="mapFrame"></param>
        void AddMapFrame(IFrame mapFrame);
    }
}