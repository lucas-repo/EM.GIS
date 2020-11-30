using System;
using System.Globalization;

namespace EM.GIS.Projection
{
    /// <summary>
    /// 子午线
    /// </summary>
    public class Meridian : BaseCopy
    {
        #region Properties

        /// <summary>
        /// Gets or sets the string name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the longitude where the prime meridian for this geographic coordinate occurs.
        /// </summary>
        public virtual double Longitude { get; set; }

        #endregion Properties

    }
}