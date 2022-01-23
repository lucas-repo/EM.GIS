using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// IntPtr扩展
    /// </summary>
    public static class IntPtrExtensions
    {
        /// <summary>
        /// 计算指定地址字节长度
        /// </summary>
        /// <param name="strPtr">地址</param>
        /// <returns>字节长度</returns>
        public static int GetIntPtrLength(this IntPtr strPtr)
        {
            int size;
            for (size = 0; Marshal.ReadByte(strPtr, size) > 0; size++) ;
            return size;
        }
        /// <summary>
        /// 从指定地址根据编码读取字符串
        /// </summary>
        /// <param name="strPtr">地址</param>
        /// <param name="encodingName">编码名称</param>
        /// <returns>字符串</returns>
        public static string IntPtrTostring(this IntPtr strPtr, string encodingName)
        {
            var encoding= Encoding.GetEncoding(encodingName);
            string value = strPtr.IntPtrTostring(encoding);
            return value;
        }
        /// <summary>
        /// 从指定地址根据编码读取字符串
        /// </summary>
        /// <param name="strPtr">地址</param>
        /// <param name="encodingName">编码名称</param>
        /// <returns>字符串</returns>
        public static string IntPtrTostring(this IntPtr strPtr, Encoding encoding)
        {
            int size = GetIntPtrLength(strPtr);
            byte[] array = new byte[size];
            Marshal.Copy(strPtr, array, 0, size);
            string value = encoding.GetString(array);
            return value;
        }
        /// <summary>
        /// 将字符串转成IntPtr
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encoding">编码</param>
        /// <returns>IntPtr</returns>
        public static IntPtr StringToIntPtr(this string str, Encoding encoding)
        {
            byte[] array = encoding.GetBytes(str);
            GCHandle hObject = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();
            if (hObject.IsAllocated)
                hObject.Free();
            return pObject;
        }
    }
}
