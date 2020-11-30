namespace EM.GIS.Symbology
{
    /// <summary>
    /// 线分类
    /// </summary>
    public interface ILineCategory : IFeatureCategory
    {
        /// <summary>
        /// 符号
        /// </summary>
        new ILineSymbolizer Symbolizer { get; set; }
        /// <summary>
        /// 选择符号
        /// </summary>
        new ILineSymbolizer SelectionSymbolizer { get; set; }
    }
}