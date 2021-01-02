using System.Drawing;
using System.Drawing.Drawing2D;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 边线符号
    /// </summary>
    public interface IOutlineSymbol:IFeatureSymbol
    {
        /// <summary>
        /// 使用边线
        /// </summary>
        bool UseOutLine { get; set; }
        /// <summary>
        /// 边线符号化
        /// </summary>
        ILineSymbolizer OutLineSymbolizer { get; set; }
        /// <summary>
        /// 复制边线
        /// </summary>
        /// <param name="outlineSymbol"></param>
        void CopyOutLine(IOutlineSymbol outlineSymbol);
        /// <summary>
        /// 绘制线
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scale"></param>
        /// <param name="path"></param>
        void DrawOutLine(Graphics context, float scale,GraphicsPath path);
    }
}