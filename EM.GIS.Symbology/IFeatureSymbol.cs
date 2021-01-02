using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素符号接口
    /// </summary>
    public interface IFeatureSymbol: ISymbol
    {
        /// <summary>
        /// 颜色
        /// </summary>
        Color Color { get; set; }
        /// <summary>
        /// 不透明度
        /// </summary>
        float Opacity { get; set; }
    }
}
