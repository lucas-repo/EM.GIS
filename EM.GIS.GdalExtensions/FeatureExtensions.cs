using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 要素扩展
    /// </summary>
    public static class FeatureExtensions
    {
        /// <summary>
        /// gdal文件名
        /// </summary>
        public const string GdalDllName = "gdal202.dll";
        /// <summary>
        /// gdal编码名称
        /// </summary>
        public const string GdalEncoding = "GBK";

        /// <summary>
        /// 获取要素指定字段的字符串值
        /// </summary>
        /// <param name="featureHandle">要素句柄</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <returns>字符串值</returns>
        [DllImport(GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_F_GetFieldAsString(HandleRef featureHandle, int fieldIndex);
        /// <summary>
        /// 获取要素的字段定义
        /// </summary>
        /// <param name="featureHandle">要素句柄</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <returns>字段定义</returns>
        [DllImport(GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_F_GetFieldDefnRef(HandleRef featureHandle, int fieldIndex);
        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="fieldDefn">字段定义</param>
        /// <returns>字段名称</returns>
        [DllImport(GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_Fld_GetNameRef(IntPtr fieldDefn);
        /// <summary>
        /// 设置要素指定字段的值为字符串
        /// </summary>
        /// <param name="featureHandle">要素句柄</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <param name="value">值</param>
        [DllImport(GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static void OGR_F_SetFieldString(HandleRef featureHandle, int fieldIndex, IntPtr value);

        /// <summary>
        /// 获取要素指定字段的字符串值
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <returns>字符串值</returns>
        public static string GetStringValue(this Feature feature, int fieldIndex)
        {
            HandleRef handle = Feature.getCPtr(feature);
            IntPtr intptr = OGR_F_GetFieldAsString(handle, fieldIndex);
            string value = intptr.IntPtrTostring(GdalEncoding);
            return value;
        }
        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <returns>字段名</returns>
        public static string GetFieldName(this Feature feature, int fieldIndex)
        {
            HandleRef handle = Feature.getCPtr(feature);
            IntPtr ptr = OGR_F_GetFieldDefnRef(handle, fieldIndex);
            IntPtr strPtr = OGR_Fld_GetNameRef(ptr);
            string value = strPtr.IntPtrTostring(GdalEncoding);
            return value;
        }
        /// <summary>
        /// 设置要素指定字段的值为字符串
        /// </summary>
        /// <param name="feature">要素</param>
        /// <param name="fieldIndex">字段索引</param>
        /// <param name="value">值</param>
        public static void OGR_F_SetFieldString(this Feature feature, int fieldIndex, string value)
        {
            HandleRef handle = Feature.getCPtr(feature);
            IntPtr intptr = Marshal.StringToHGlobalAnsi(value);
            OGR_F_SetFieldString(handle, fieldIndex, intptr);
        }

        public static object GetFieldValue(this Feature feature, int index)
        {
            if (feature == null || index < 0 || index >= feature.GetFieldCount())
            {
                throw new Exception("参数设置错误");
            }
            object value;
            using (var fieldDefn = feature.GetFieldDefnRef(index))
            {
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
            }
            return value;
        }
        public static object GetFieldValue(this Feature feature, string fieldName)
        {
            if (feature == null || string.IsNullOrEmpty(fieldName))
            {
                throw new Exception("参数设置错误");
            }
            object value;
            using (var fieldDefn = feature.GetFieldDefnRef(fieldName))
            {
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
                    count = geometry.GetTotalPointCount();
                }
            }
            return count;
        }
    }
}
