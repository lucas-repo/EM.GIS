using EM.GIS.Data;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格图层
    /// </summary>
    public interface ITileLayer:IRasterLayer 
    {
        /// <summary>
        /// 数据集
        /// </summary>
        new ITileSet? DataSet { get; set; }
    }
}
