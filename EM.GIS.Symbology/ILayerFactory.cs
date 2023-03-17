using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层工厂
    /// </summary>
    public interface ILayerFactory
    {
        /// <summary>
        /// 创建要素图层
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="featureType">要素类型</param>
        /// <returns>要素图层</returns>
        IFeatureLayer? CreateFeatureLayer(string name, FeatureType featureType);
    }
}
