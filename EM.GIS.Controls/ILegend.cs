using EM.Bases;
using EM.GIS.Symbology;
using System.Collections.ObjectModel;

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
        ObservableCollection<ITreeItem> LegendItems { get; }
        /// <summary>
        /// 添加地图框架
        /// </summary>
        /// <param name="mapFrame"></param>
        void AddMapFrame(IFrame mapFrame);
    }
}