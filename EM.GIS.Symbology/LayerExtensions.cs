using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图层扩展
    /// </summary>
    public static class LayerExtensions
    {
        /// <summary>
        /// 获取进度字符串
        /// </summary>
        /// <param name="legendItem">图例元素</param>
        /// <returns>进度字符串</returns>
        public static string GetProgressString(this ILegendItem legendItem)
        {
            string ret=string.Empty;
            if (legendItem != null)
            {
                ret= $"绘制 {legendItem?.Text} 中...";
            }
            return ret;
        }
    }
}
