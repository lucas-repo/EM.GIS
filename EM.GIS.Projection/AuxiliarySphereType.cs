namespace EM.GIS.Projection
{
    public enum AuxiliarySphereType
    {
        /// <summary>
        /// Use semimajor axis or radius of the geographic coordinate system
        /// </summary>
        SemimajorAxis = 0,

        /// <summary>
        /// Use semiminor axis or radius
        /// </summary>
        SemiminorAxis = 1,

        /// <summary>
        /// Calculate and use authalic radius
        /// </summary>
        Authalic = 2,

        /// <summary>
        /// Use authalic radius and convert geodetic latitudes to authalic latitudes
        /// </summary>
        AuthalicWithConvertedLatitudes = 3,

        /// <summary>
        /// This indicates that this parameter should not appear in the projection string.
        /// </summary>
        NotSpecified = 4
    }
}