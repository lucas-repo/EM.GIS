using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点符号
    /// </summary>
    public interface IPointSymbol: IFeatureSymbol,IOutlineSymbol
    {
        /// <summary>
        /// 角度
        /// </summary>
        float Angle { get; set; }
        /// <summary>
        /// 偏移
        /// </summary>
        PointF Offset { get; set; }
        /// <summary>
        /// 点符号类型
        /// </summary>
        PointSymbolType PointSymbolType { get; }
        /// <summary>
        /// 绘制点
        /// </summary>
        /// <param name="context"></param>
        /// <param name="scale"></param>
        /// <param name="point"></param>
        void DrawPoint(Graphics context, float scale, PointF point);
        /// <summary>
        /// 尺寸
        /// </summary>
        SizeF Size { get; set; }
    }
}
