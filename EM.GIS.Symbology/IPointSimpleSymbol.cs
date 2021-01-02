namespace EM.GIS.Symbology
{
    /// <summary>
    /// 简单点符号
    /// </summary>
    public interface IPointSimpleSymbol : IPointSymbol
    {
        /// <summary>
        /// 点形状
        /// </summary>
        PointShape PointShape { get; set; }
    }
}