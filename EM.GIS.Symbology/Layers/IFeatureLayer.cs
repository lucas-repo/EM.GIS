using EM.GIS.Data;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素图层集合
    /// </summary>
    public interface IFeatureLayer: ILayer
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new IFeatureCategoryCollection Children { get; }

        /// <summary>
        /// 数据集
        /// </summary>
        new IFeatureSet? DataSet { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        IFeatureSelection Selection { get; }
        /// <summary>
        /// 标注图层
        /// </summary>
        ILabelLayer LabelLayer { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        IFeatureCategory DefaultCategory { get; set; }
        /// <summary>
        /// 绘制缓存（要素Id和要素分类）集合
        /// </summary>
        Dictionary<long, IFeatureCategory> DrawnStates { get; }
    }
}
