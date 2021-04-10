using EM.GIS.Data;
using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class FeaturesetExtensions
    {
        public static FeatureType ToFeatureType(this  wkbGeometryType geometryType)
        {
            FeatureType featureType = FeatureType.Unknown;
            switch (geometryType)
            {
                case OSGeo.OGR.wkbGeometryType.wkbPoint:
                case OSGeo.OGR.wkbGeometryType.wkbMultiPoint:
                    featureType = FeatureType.Point;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbLineString:
                case OSGeo.OGR.wkbGeometryType.wkbMultiLineString:
                    featureType = FeatureType.Polyline;
                    break;
                case OSGeo.OGR.wkbGeometryType.wkbPolygon:
                case OSGeo.OGR.wkbGeometryType.wkbMultiPolygon:
                    featureType =  FeatureType.Polygon;
                    break;
                default:
                    throw new NotImplementedException("暂未实现的类型");
            }
            return featureType;
        }
    }
}
