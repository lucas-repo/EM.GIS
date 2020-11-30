namespace EM.GIS.Symbology
{
    /// <summary>
    /// Interface for ImageSymbolizer.
    /// </summary>
    public interface IImageSymbolizer : ILegendItem
    {
        /// <summary>
        /// Gets or sets a float value from 0 to 1, where 1 is fully opaque while 0 is fully transparent.
        /// </summary>
        float Opacity { get; set; }
    }
}