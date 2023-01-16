using EM.Bases;

namespace EM.GIS.Projections
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
        /// 长半轴
        /// </summary>
        public virtual double Semimajor { get;  }

        /// <summary>
        /// 短半轴
        /// </summary>
        public virtual double Semiminor { get; }
        #endregion Properties
    }
}