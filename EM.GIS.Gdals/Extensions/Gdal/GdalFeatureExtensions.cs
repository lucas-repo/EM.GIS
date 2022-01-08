using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 要素扩展
    /// </summary>
    public static class GdalFeatureExtensions
    {
        public static object GetFieldValue(this Feature feature, int index)
        {
            if (feature == null || index < 0 || index >= feature.GetFieldCount())
            {
                throw new Exception("参数设置错误");
            }
            using var fieldDefn = feature.GetFieldDefnRef(index);
            object value;
            switch (fieldDefn.GetFieldType())
            {
                case OSGeo.OGR.FieldType.OFTString:
                    value = feature.GetFieldAsString(index);
                    break;
                case OSGeo.OGR.FieldType.OFTInteger:
                    value = feature.GetFieldAsInteger(index);
                    break;
                case OSGeo.OGR.FieldType.OFTInteger64:
                    value = feature.GetFieldAsInteger64(index);
                    break;
                case OSGeo.OGR.FieldType.OFTReal:
                    value = feature.GetFieldAsDouble(index);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return value;
        }
        public static object GetFieldValue(this Feature feature, string fieldName)
        {
            if (feature == null || string.IsNullOrEmpty(fieldName))
            {
                throw new Exception("参数设置错误");
            }
            using var fieldDefn = feature.GetFieldDefnRef(fieldName);
            object value;
            switch (fieldDefn.GetFieldType())
            {
                case OSGeo.OGR.FieldType.OFTString:
                    value = feature.GetFieldAsString(fieldName);
                    break;
                case OSGeo.OGR.FieldType.OFTInteger:
                    value = feature.GetFieldAsInteger(fieldName);
                    break;
                case OSGeo.OGR.FieldType.OFTInteger64:
                    value = feature.GetFieldAsInteger64(fieldName);
                    break;
                case OSGeo.OGR.FieldType.OFTReal:
                    value = feature.GetFieldAsDouble(fieldName);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return value;
        }
        public static void SetField(this Feature feature, OSGeo.OGR.FieldDefn fieldDefn, object value)
        {
            if (feature == null || fieldDefn == null)
            {
                return;
            }
            var fieldName = fieldDefn.GetName();
            var fieldType = fieldDefn.GetFieldType();
            if (!DBNull.Value.Equals(value))
            {
                switch (fieldType)
                {
                    case OSGeo.OGR.FieldType.OFTString:
                        feature.SetField(fieldName, value.ToString());
                        break;
                    case OSGeo.OGR.FieldType.OFTInteger:
                        if (value is int intValue)
                        {
                            feature.SetField(fieldName, intValue);
                        }
                        break;
                    case OSGeo.OGR.FieldType.OFTReal:
                        if (value is double doubleValue)
                        {
                            feature.SetField(fieldName, doubleValue);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public static int GetPointCount(this Feature feature)
        {
            int count = 0;
            if (feature != null)
            {
                using (var geometry = feature.GetGeometryRef())
                {
                    geometry.GetPointCount(ref count);
                }
            }
            return count;
        }
    }
}
