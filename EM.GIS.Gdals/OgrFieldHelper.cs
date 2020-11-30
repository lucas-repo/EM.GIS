using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class OgrFieldHelper
    {
        [DllImport("gdal202.dll", EntryPoint = "OGR_F_GetFieldAsString")]
        public extern static IntPtr OGR_F_GetFieldAsString(HandleRef handleRef, int i);
        /// <summary>
        /// 读取要素的字段值
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="fieldIndex"></param>
        /// <returns></returns>
        public static string GetStringValue(this Feature feature, int fieldIndex)
        {
            string value = string.Empty;
            if (feature == null || fieldIndex == -1)
            {
                return value;
            }
            IntPtr intPtr = OGR_F_GetFieldAsString(Feature.getCPtr(feature), fieldIndex);
            List<byte> buffer = new List<byte>();
            int offset = 0;
            byte byteValue= Marshal.ReadByte(intPtr, offset);
            while (byteValue != 0)
            {
                buffer.Add(byteValue);
                offset++;
                byteValue = Marshal.ReadByte(intPtr, offset);
            }
            value = Encoding.UTF8.GetString(buffer.ToArray());
            return value;
        }
    }
}
