namespace EM.GIS.Data
{
    /// <summary>
    /// An abreviated list for quick classification
    /// </summary>
    public enum FeatureType
    {
        /// <summary>
        /// None specified or custom
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Point
        /// </summary>
        Point = 1,

        /// <summary>
        /// Line
        /// </summary>
        Line = 2,

        /// <summary>
        /// Polygon
        /// </summary>
        Polygon = 3,

        /// <summary>
        /// MultiPoint
        /// </summary>
        MultiPoint = 4
    }
}