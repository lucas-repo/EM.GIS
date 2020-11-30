using EM.GIS.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class FeatureExtensions
    {
        #region 要素操作
        public static IFeature ToFeature(this OSGeo.OGR.Feature feature)
        {
            IFeature destFeature = null;
            if (feature != null)
            {
                destFeature = new GdalFeature(feature);
            }
            return destFeature;
        }
        public static OSGeo.OGR.Feature ToFeature(this IFeature feature)
        {
            OSGeo.OGR.Feature destFeature = null;
            if (feature is GdalFeature gdalFeature)
            {
                destFeature = gdalFeature.Feature;
            }
            return destFeature;
        }
        public static object GetFieldValue(this OSGeo.OGR.Feature feature, int index)
        {
            if (feature == null || index < 0 || index >= feature.GetFieldCount())
            {
                throw new Exception("参数设置错误");
            }
            using var fieldDefn = feature.GetFieldDefnRef(index);
            object value;
            switch (fieldDefn.GetFieldType())
            {
                case  OSGeo.OGR.FieldType.OFTString:
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
        public static object GetFieldValue(this OSGeo.OGR.Feature feature, string fieldName)
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
        public static void SetField(this OSGeo.OGR.Feature feature, OSGeo.OGR.FieldDefn fieldDefn, object value)
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
        public static FieldType ToFieldType(this OSGeo.OGR.FieldType fieldType)
        {
            FieldType destFieldType = FieldType.Unkown;
            switch (fieldType)
            {
                case OSGeo.OGR.FieldType.OFTInteger:
                    destFieldType = FieldType.Int;
                    break;
                case OSGeo.OGR.FieldType.OFTIntegerList:
                    destFieldType = FieldType.IntList;
                    break;
                case OSGeo.OGR.FieldType.OFTReal:
                    destFieldType = FieldType.Double;
                    break;
                case OSGeo.OGR.FieldType.OFTRealList:
                    destFieldType = FieldType.DoubleList;
                    break;
                case OSGeo.OGR.FieldType.OFTString:
                    destFieldType = FieldType.String;
                    break;
                case OSGeo.OGR.FieldType.OFTStringList:
                    destFieldType = FieldType.StringList;
                    break;
                case OSGeo.OGR.FieldType.OFTWideString:
                    destFieldType = FieldType.WideString;
                    break;
                case OSGeo.OGR.FieldType.OFTWideStringList:
                    destFieldType = FieldType.WideStringList;
                    break;
                case OSGeo.OGR.FieldType.OFTBinary:
                    destFieldType = FieldType.Binary;
                    break;
                case OSGeo.OGR.FieldType.OFTDateTime:
                    destFieldType = FieldType.DateTime;
                    break;
                case OSGeo.OGR.FieldType.OFTInteger64:
                    destFieldType = FieldType.Long;
                    break;
                case OSGeo.OGR.FieldType.OFTInteger64List:
                    destFieldType = FieldType.LongList;
                    break;
            }
            return destFieldType;
        }
        public static  OSGeo.OGR.FieldType ToFieldType(this FieldType fieldType)
        {
            if (fieldType == FieldType.Unkown)
            {
                throw new NotImplementedException();
            }
            OSGeo.OGR.FieldType destFieldType = OSGeo.OGR.FieldType.OFTString;
            switch (fieldType)
            {
                case FieldType.Int:
                    destFieldType = OSGeo.OGR.FieldType.OFTInteger;
                    break;
                case FieldType.IntList:
                    destFieldType = OSGeo.OGR.FieldType.OFTIntegerList;
                    break;
                case FieldType.Double:
                    destFieldType = OSGeo.OGR.FieldType.OFTReal;
                    break;
                case FieldType.DoubleList:
                    destFieldType = OSGeo.OGR.FieldType.OFTRealList;
                    break;
                case FieldType.String:
                    destFieldType = OSGeo.OGR.FieldType.OFTString;
                    break;
                case FieldType.StringList:
                    destFieldType = OSGeo.OGR.FieldType.OFTStringList;
                    break;
                case FieldType.WideString:
                    destFieldType = OSGeo.OGR.FieldType.OFTWideString;
                    break;
                case FieldType.WideStringList:
                    destFieldType = OSGeo.OGR.FieldType.OFTWideStringList;
                    break;
                case FieldType.Binary:
                    destFieldType = OSGeo.OGR.FieldType.OFTBinary;
                    break;
                case FieldType.DateTime:
                    destFieldType = OSGeo.OGR.FieldType.OFTDateTime;
                    break;
                case FieldType.Long:
                    destFieldType = OSGeo.OGR.FieldType.OFTInteger64;
                    break;
                case FieldType.LongList:
                    destFieldType = OSGeo.OGR.FieldType.OFTInteger64List;
                    break;
            }
            return destFieldType;
        }
        #endregion
    }
}
