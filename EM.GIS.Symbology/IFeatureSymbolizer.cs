using EM.GIS.Data;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 要素符号化接口
    /// </summary>
    public interface IFeatureSymbolizer:ISymbolizer
    {
        /// <summary>
        /// 缩放模式
        /// </summary>
        ScaleMode ScaleMode { get; set; }
        /// <summary>
        /// 获取缩放比例
        /// </summary>
        /// <param name="drawArgs">投影接口</param>
        /// <returns>缩放比例</returns>
        double GetScale(IProj drawArgs);
        /// <summary>
        /// 符号集合
        /// </summary>
        IFeatureSymbolCollection Symbols { get; set; }
    }
}
