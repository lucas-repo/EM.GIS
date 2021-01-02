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

        double GetScale(MapArgs drawArgs);
        /// <summary>
        /// 符号集合
        /// </summary>
        IFeatureSymbolCollection Symbols { get; set; }
    }
}
