using EM.GIS.Data;

namespace EM.GIS.Symbology
{
    public interface IRasterLayer : ILayer
    {
        /// <summary>
        /// 栅格数据集
        /// </summary>
        new IRasterSet DataSet { get; set; }
        /// <summary>
        /// 分类集合
        /// </summary>
        new IRasterCategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new IRasterCategory DefaultCategory { get; set; }

    }
}
