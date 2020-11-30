namespace EM.GIS.Symbology
{
    /// <summary>
    /// 点分类
    /// </summary>
    public interface IPointCategory : IFeatureCategory
    {
        /// <summary>
        /// 选择符号
        /// </summary>
        new IPointSymbolizer SelectionSymbolizer { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        new IPointSymbolizer Symbolizer { get; set; }
    }
}