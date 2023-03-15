using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 矩形扩展类
    /// </summary>
    public static class RectangleExtensions
    {
        /// <summary>
        /// 合并两个矩形
        /// </summary>
        /// <param name="rect0">矩形0</param>
        /// <param name="rect1">矩形1</param>
        /// <returns>合并后的矩形</returns>
        public static RectangleF Union(this RectangleF rect0, RectangleF rect1)
        {
            RectangleF ret = RectangleF.Empty;
            if (rect0.IsEmpty)
            {
                if (!rect1.IsEmpty)
                {
                    ret = rect1;
                }
            }
            else
            {
                if (rect1.IsEmpty)
                {
                    ret = rect0;
                }
                else
                {
                    ret = RectangleF.FromLTRB(Math.Min(rect0.Left, rect1.Left), Math.Min(rect0.Top, rect1.Top), Math.Max(rect0.Right, rect1.Right), Math.Max(rect0.Bottom, rect1.Bottom));
                }
            }
            return ret;
        }
    }
}
