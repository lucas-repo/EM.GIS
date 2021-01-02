using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线符号接口
    /// </summary>
    public interface ILineSymbol : IFeatureSymbol
    {
        /// <summary>
        /// 宽度
        /// </summary>
        float Width { get; set; }
        /// <summary>
        /// 线符号类型
        /// </summary>
        LineSymbolType LineSymbolType { get; }
        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scale"></param>
        /// <param name="path"></param>
        void DrawLine(Graphics context, float scale, GraphicsPath path);
        /// <summary>
        /// 转成Pen
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        Pen ToPen(float scale);
    }
}
