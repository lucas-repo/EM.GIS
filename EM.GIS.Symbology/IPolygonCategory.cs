namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面分类
    /// </summary>
    public interface IPolygonCategory:IFeatureCategory
    {
        /// <summary>
        /// 选择符号
        /// </summary>
        new IPolygonSymbolizer SelectionSymbolizer { get; set; }
        /// <summary>
        /// 符号
        /// </summary>
        new IPolygonSymbolizer Symbolizer { get; set; }
    }
}