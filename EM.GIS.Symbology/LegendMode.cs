namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例类型
    /// </summary>
    public enum LegendMode
    {
        /// <summary>
        /// Display a checkbox next to the legend item
        /// </summary>
        Checkbox,

        /// <summary>
        /// Draws a symbol, but also allows collapsing.
        /// </summary>
        GroupSymbol,

        /// <summary>
        /// Display a symbol next to the legend item
        /// </summary>
        Symbol,

        /// <summary>
        /// Display only legend text
        /// </summary>
        None
    }
}