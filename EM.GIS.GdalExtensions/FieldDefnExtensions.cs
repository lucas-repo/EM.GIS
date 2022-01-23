using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class FieldDefnExtensions
    {
        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="fieldDefn">字段定义</param>
        /// <returns>字段名称</returns>
        [DllImport(FeatureExtensions.GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_Fld_GetNameRef(IntPtr fieldDefn);
        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="fieldDefn">字段定义</param>
        /// <returns>字段名称</returns>
        [DllImport(FeatureExtensions.GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_Fld_GetNameRef(HandleRef fieldDefn);
    
        /// <summary>
        /// 获取字段名称
        /// </summary>
        /// <param name="feature">字段定义</param>
        /// <returns>字段名</returns>
        public static string GetNameUTF8(this FieldDefn fieldDefn)
        {
            var fieldDefnRef = FieldDefn.getCPtr(fieldDefn);
            IntPtr strPtr = OGR_Fld_GetNameRef(fieldDefnRef);
            string value = strPtr.IntPtrTostring(Encoding.UTF8);
            return value;
        }
        /// <summary>
        /// 复制字段定义
        /// </summary>
        /// <param name="srcFieldDefn">源字段定义</param>
        /// <returns>字段定义</returns>
        public static FieldDefn Clone(this FieldDefn srcFieldDefn)
        {
            FieldDefn destFieldDefn =null;
            if (srcFieldDefn!=null)
            {
                var fieldName=srcFieldDefn.GetName();
                var fieldType = srcFieldDefn.GetFieldType();
                destFieldDefn = new FieldDefn(fieldName, fieldType);

                #region 临时
                var src0= srcFieldDefn.GetName();
                var src1 = srcFieldDefn.GetNameRef();
                var src2 = srcFieldDefn.GetNameUTF8();
                //var bytes = Encoding.UTF8.GetBytes(src1);
                //var destBytes= Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("GBK"), bytes);
                //var destFieldName = Encoding.GetEncoding("GBK").GetString(destBytes);
                destFieldDefn.SetName("中文字");
                var dest0 = destFieldDefn.GetName();
                var dest1 = destFieldDefn.GetNameUTF8();

                #endregion


                destFieldDefn.SetNullable(srcFieldDefn.IsNullable());
                destFieldDefn.SetWidth(srcFieldDefn.GetWidth());
                destFieldDefn.SetPrecision(srcFieldDefn.GetPrecision());
                destFieldDefn.SetIgnored(srcFieldDefn.IsIgnored());
                destFieldDefn.SetJustify(srcFieldDefn.GetJustify());
                destFieldDefn.SetSubType(srcFieldDefn.GetSubType());
                if (srcFieldDefn.IsDefaultDriverSpecific()==1)
                {
                    destFieldDefn.SetDefault(srcFieldDefn.GetDefault()); 
                }
            }
            return destFieldDefn;
        }
    }
}
