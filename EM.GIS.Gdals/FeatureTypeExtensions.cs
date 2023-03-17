using EM.GIS.Data;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 要素类型扩展类
    /// </summary>
    public static class FeatureTypeExtensions
    {
        /// <summary>
        /// 将<see cref="FeatureType"/> 转为<see cref="wkbGeometryType"/>
        /// </summary>
        /// <param name="featureType">要素类型</param>
        /// <returns>几何类型</returns>
        public static wkbGeometryType ToWkbGeometryType(this FeatureType featureType)
        {
            wkbGeometryType ret = wkbGeometryType.wkbUnknown;
            switch (featureType)
            {
                case FeatureType.Point:
                case FeatureType.MultiPoint:
                    ret = wkbGeometryType.wkbPoint;
                    break;
                case FeatureType.Polyline:
                    ret = wkbGeometryType.wkbLineString;
                    break;
                case FeatureType.Polygon:
                    ret = wkbGeometryType.wkbPolygon;
                    break;
            }
            return ret;
        }
    }
}
