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
        new IFeatureCategory Category { get; set; }
        /// <summary>
        /// 选择范围里的要素
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="affectedExtent"></param>
        /// <returns></returns>
        bool AddRegion(IExtent extent, out IExtent affectedExtent);
        /// <summary>
        /// 反选范围里的要素
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="affectedExtent"></param>
        /// <returns></returns>
        bool InvertSelection(IExtent extent, out IExtent affectedExtent);
        /// <summary>
        /// 移除范围里选择的要素
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="affectedExtent"></param>
        /// <returns></returns>
        bool RemoveRegion(IExtent extent, out IExtent affectedExtent);

    }
}