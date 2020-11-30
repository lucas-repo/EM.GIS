using System;

namespace EM.GIS.Projection
{
    /// <summary>
    /// 地理信息
    /// </summary>
    public class GeographicInfo : BaseCopy
    {
        #region Properties

        /// <summary>
        /// Gets or sets the datum
        /// eg: D_WGS_1984
        /// </summary>
        public virtual Datum Datum { get; set; }

        /// <summary>
        /// Gets or sets the prime meridian longitude of the 0 mark, relative to Greenwitch
        /// </summary>
        public virtual Meridian Meridian { get; set; }

        /// <summary>
        /// Gets or sets the geographic coordinate system name
        /// eg: GCS_WGS_1984
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the units
        /// </summary>
        public virtual AngularUnit Unit { get; set; }

        #endregion

    }
}