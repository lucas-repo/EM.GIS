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
        public static string GetFieldAsStringUTF8(this Feature feature, int fieldIndex)
        {
            HandleRef handle = Feature.getCPtr(feature);
            IntPtr intptr = OGR_F_GetFieldAsString(handle, fieldIndex);
            string value = intptr.IntPtrTostring(Encoding.UTF8);
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
            HandleRef handleRef = Feature.getCPtr(feature);
            //IntPtr intptr = Marshal.StringToHGlobalAnsi(value);
            byte[] srcBytes = Encoding.UTF8.GetBytes(value);
            byte[] destBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(GdalEncoding), srcBytes);
            var destStr = Encoding.GetEncoding(GdalEncoding).GetString(destBytes);
            GCHandle handle = GCHandle.Alloc(destBytes, GCHandleType.Pinned);
            IntPtr intptr = handle.AddrOfPinnedObject();
            OGR_F_SetFieldString(handleRef, fieldIndex, intptr);
            if (handle.IsAllocated)
                handle.Free();

            // Always free the unmanaged string.
            //Marshal.FreeHGlobal(intptr);
        }
        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="destFeature">目标要素</param>
        /// <param name="srcFeature">字段值来源要素</param>
        /// <param name="fieldName">字段名称</param>
        public static void SetField(this Feature destFeature, Feature srcFeature, string fieldName)
        {
            if (destFeature!=null&&srcFeature!=null)
            {
                var destFieldIndex = destFeature.GetDefnRef().GetFieldIndexUTF8(fieldName);
                var srcFieldIndex = srcFeature.GetDefnRef().GetFieldIndexUTF8(fieldName);
                if (destFieldIndex!=-1&&srcFieldIndex!=-1)
                {
                    destFeature.SetField(destFieldIndex, srcFeature, srcFieldIndex);
                }
            }
        }
        /// <summary>
        /// 设置字段值
        /// </summary>
        /// <param name="destFeature">目标要素</param>
        /// <param name="destFieldIndex">目标字段索引</param>
        /// <param name="srcFeature">字段值来源要素</param>
        /// <param name="srcFieldIndex">来源字段索引</param>
        public static void SetField(this Feature destFeature, int destFieldIndex, Feature srcFeature, int srcFieldIndex)
        {
            if (destFieldIndex<0||srcFieldIndex<0)
            {
                return;
            }
            var destFieldDefn = destFeature?.GetFieldDefnRef(destFieldIndex);
            var srcFieldFromDefn = srcFeature?.GetFieldDefnRef(srcFieldIndex);
            if (destFieldDefn!=null&&srcFieldFromDefn!=null)
            {
                var fieldType = destFieldDefn.GetFieldType();
                if (fieldType==srcFieldFromDefn.GetFieldType())
                {
                    int count;
                    switch (fieldType)
                    {
                        case FieldType.OFTInteger:
                            destFeature.SetField(destFieldIndex, srcFeature.GetFieldAsInteger(srcFieldIndex));
                            break;
                        case FieldType.OFTIntegerList:
                            var ints = srcFeature.GetFieldAsIntegerList(srcFieldIndex, out count);
                            destFeature.SetFieldIntegerList(destFieldIndex, count, ints);
                            break;
                        case FieldType.OFTReal:
                            destFeature.SetField(destFieldIndex, srcFeature.GetFieldAsDouble(srcFieldIndex));
                            break;
                        case FieldType.OFTRealList:
                        case FieldType.OFTInteger64List:
                            var doubles = srcFeature.GetFieldAsDoubleList(srcFieldIndex, out count);
                            destFeature.SetFieldDoubleList(destFieldIndex, count, doubles);
                            break;
                        case FieldType.OFTString:
                            var str1 = srcFeature.GetFieldAsString(srcFieldIndex);
                            var str = srcFeature.GetFieldAsStringUTF8(srcFieldIndex);
                            //OGR_F_SetFieldString(destFeature, destFieldIndex, str);
                            destFeature.SetField(destFieldIndex, str);
                            //Feature_SetField__SWIG_0(destFeature, destFieldIndex, str);
                            break;
                        case FieldType.OFTStringList:
                            var strings = srcFeature.GetFieldAsStringList(srcFieldIndex);
                            destFeature.SetFieldStringList(destFieldIndex, strings);
                            break;
                        case FieldType.OFTDate:
                        case FieldType.OFTTime:
                        case FieldType.OFTDateTime:
                            srcFeature.GetFieldAsDateTime(srcFieldIndex, out int year, out int month, out int day, out int hour, out int minute, out float second, out int tzflag);
                            destFeature.SetField(destFieldIndex, year, month, day, hour, minute, second, tzflag);
                            break;
                        case FieldType.OFTInteger64:
                            destFeature.SetField(destFieldIndex, srcFeature.GetFieldAsInteger64(srcFieldIndex));
                            break;
                        default:
                            throw new NotImplementedException("无相应封装方法");
                    }
                }
            }
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
