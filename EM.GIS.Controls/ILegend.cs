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
        ILegendItemCollection Items { get; }
    }
}