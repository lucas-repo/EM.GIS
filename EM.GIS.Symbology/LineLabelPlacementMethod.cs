namespace EM.GIS.Symbology
{
    /// <summary>
    /// Methods used to calculate the placement of line labels.
    /// </summary>
    public enum LineLabelPlacementMethod
    {
        /// <summary>
        /// Uses the longest segment of the LineString.
        /// </summary>
        LongestSegment,

        /// <summary>
        /// Uses the first segment of the LineString.
        /// </summary>
        FirstSegment,

        /// <summary>
        /// Uses the middle segment of the LineString.
        /// </summary>
        MiddleSegment,

        /// <summary>
        /// Uses the last segment of the LineString.
        /// </summary>
        LastSegment
    }
}