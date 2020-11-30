namespace EM.GIS.Projection
{
    /// <summary>
    /// Represents possible datum types.
    /// </summary>
    public enum DatumType
    {
        /// <summary>
        /// The datum type is not with a well defined ellips or grid-shift
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The datum transform to WGS84 can be defined using 3 double parameters
        /// </summary>
        Param3 = 1,

        /// <summary>
        /// The datum transform to WGS84 can be defined using 7 double parameters
        /// </summary>
        Param7 = 2,

        /// <summary>
        /// The transform requires a special nad gridshift
        /// </summary>
        GridShift,

        /// <summary>
        /// The datum is already the WGS84 datum
        /// </summary>
        WGS84
    }
}