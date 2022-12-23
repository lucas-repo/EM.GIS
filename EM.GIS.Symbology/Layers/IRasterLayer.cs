using EM.GIS.Data;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 栅格图层
    /// </summary>
    public interface IRasterLayer : ILayer
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new IRasterCategoryCollection Children { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new IRasterCategory DefaultCategory { get; set; }

    }
}
