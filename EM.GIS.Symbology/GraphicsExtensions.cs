using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 绘制扩展类
    /// </summary>
    public static class GraphicsExtensions
    {
        /// <summary>
        /// 获取路径的范围
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static RectangleF GetRectangle(GraphicsPath path)
        {
            float left = float.NaN;
            float top = float.NaN;
            float right = float.NaN;
            float bottom = float.NaN;
            foreach (var item in path.PathPoints)
            {
                if (float.IsNaN(left))
                {
                    left = item.X;
                }
                else
                {
                    left = Math.Min(left, item.X);
                }
                if (float.IsNaN(top))
                {
                    top = item.Y;
                }
                else
                {
                    top = Math.Min(top, item.Y);
                }
                if (float.IsNaN(right))
                {
                    right = item.X;
                }
                else
                {
                    right = Math.Max(right, item.X);
                }
                if (float.IsNaN(bottom))
                {
                    bottom = item.Y;
                }
                else
                {
                    bottom = Math.Max(bottom, item.Y);
                }
            }
            RectangleF ret;
            if (float.IsNaN(left) || float.IsNaN(top) || float.IsNaN(right) || float.IsNaN(bottom))
            {
                ret = RectangleF.Empty;
            }
            else
            {
                ret=RectangleF.FromLTRB(left, top, right, bottom);
            }
            return ret;
        }
    }
}
