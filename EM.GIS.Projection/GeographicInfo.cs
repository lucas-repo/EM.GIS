using EM.Bases;
using System;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 地理信息
    /// </summary>
    public class GeographicInfo : BaseCopy
    {
        #region Properties

        /// <summary>
        /// 基准面
        /// </summary>
        public virtual Datum Datum { get; set; }

        /// <summary>
        /// 子午线
        /// </summary>
        public virtual Meridian Meridian { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 角度单位
        /// </summary>
        public virtual AngularUnit Unit { get; set; }

        #endregion

    }
}