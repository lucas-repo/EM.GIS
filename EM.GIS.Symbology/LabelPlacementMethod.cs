namespace EM.GIS.Symbology
{
    /// <summary>
    /// Methods used in calculating the placement of a label.
    /// </summary>
    public enum LabelPlacementMethod
    {
        /// <summary>
        /// Use the centroid of the feature.
        /// </summary>
        Centroid,

        /// <summary>
        /// Use the center of the extents of the feature.
        /// </summary>
        Center,

        /// <summary>
        /// Use the closest point to the centroid that is in the feature.
        /// </summary>
        InteriorPoint
    }
}