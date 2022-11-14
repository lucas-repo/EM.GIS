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
        new IFeatureSet DataSet { get; set; }
        /// <summary>
        /// 选择器
        /// </summary>
        new IFeatureSelection Selection { get; }
        /// <summary>
        /// 标注图层
        /// </summary>
        ILabelLayer LabelLayer { get; set; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new IFeatureCategory DefaultCategory { get; set; }
        /// <summary>
        /// 要素Id和分类缓存字典
        /// </summary>
        Dictionary<long, IFeatureCategory> FidCategoryDic { get; }
    }
}
