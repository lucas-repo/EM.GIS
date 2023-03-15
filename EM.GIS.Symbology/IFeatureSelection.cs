using EM.GIS.Data;
using EM.GIS.Geometries;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素选择器
    /// </summary>
    public interface IFeatureSelection:ISelection,IList<IFeature>
    {
        /// <summary>
        /// 分类
        /// </summary>
        new IFeatureCategory? Category { get; set; }
        /// <summary>
        /// 批量移除
        /// </summary>
        /// <param name="features">要素集合</param>
        void RemoveRange(IEnumerable<IFeature> features);
    }
}