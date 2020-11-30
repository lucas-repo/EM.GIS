namespace EM.GIS.Projection
{
    /// <summary>
    /// 椭球体
    /// </summary>
    public class Spheroid: BaseCopy
    {
        #region Properties

        /// <summary>
        /// Sets the value by using the current semi-major axis (Equatorial Radius) in order to
        /// calculate the semi-minor axis (Polar Radius).
        /// </summary>
        public virtual double InverseFlattening { get; set; }

        /// <summary>
        /// Gets or sets the string name of the spheroid.
        /// e.g.: WGS_1984
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// A spheroid is a pole flattened (oblate) sphere, with the radii of two axes being equal and longer
        /// than the third.  This is the radial measure of one of these major axes in meters.
        /// e.g. 6, 378, 137 for WGS 84
        /// </summary>
        public virtual double EquatorialRadius { get;  }

        /// <summary>
        /// A spheroid is a pole flattened (oblate) sphere, with the radii of two axes being equal and longer
        /// than the third.  This is the radial measure of the smaller polar axis in meters.  One option is
        /// to specify this directly, but it can also be calculated using the major axis and the flattening factor.
        /// </summary>
        public virtual double PolarRadius { get; }


        #endregion Properties
    }
}