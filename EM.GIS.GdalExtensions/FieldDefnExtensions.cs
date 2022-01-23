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
    }
}
