using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Projection
{
    /// <summary>
    /// 投影信息
    /// </summary>
    public abstract class ProjectionInfo : BaseCopy,IDisposable
    {
        public bool IsDisposed { get; private set; }
        
        /// <summary>
        ///   Gets or sets the athority, for example EPSG
        /// </summary>
        public virtual string Authority { get; set; }

        /// <summary>
        /// Gets or sets the athority code.
        /// </summary>
        public virtual string AuthorityCode { get; set; }
        public int? EPSG
        {
            get
            {
                int? epsg = null;
                if (int.TryParse(AuthorityCode, out int value))
                {
                    epsg = value;
                }
                return epsg;
            }
            set
            {
                AuthorityCode = value.ToString();
            }
        }

        /// <summary>
        ///   Gets or sets the auxiliary sphere type.
        /// </summary>
        public virtual AuxiliarySphereType AuxiliarySphereType { get; } = AuxiliarySphereType.NotSpecified;

        /// <summary>
        ///   The horizontal 0 point in geographic terms
        /// </summary>
        public virtual double? CentralMeridian { get; set; }

        /// <summary>
        ///   The false easting for this coordinate system
        /// </summary>
        public virtual double? FalseEasting { get; set; }

        /// <summary>
        ///   The false northing for this coordinate system
        /// </summary>
        public virtual double? FalseNorthing { get; set; }

        /// <summary>
        ///   Gets or sets a boolean indicating a geocentric latitude parameter
        /// </summary>
        public virtual bool Geoc { get; set; }

        /// <summary>
        ///   The geographic information
        /// </summary>
        public virtual GeographicInfo GeographicInfo { get; set; }

        /// <summary>
        ///   Gets or sets a boolean that indicates whether or not this
        ///   projection is geocentric.
        /// </summary>
        public virtual bool IsGeocentric { get; set; }

        /// <summary>
        ///   True if this coordinate system is expressed only using geographic coordinates
        /// </summary>
        public virtual bool IsLatLon { get; }

        /// <summary>
        ///   Gets or sets a boolean indicating whether this projection applies to the
        ///   southern coordinate system or not.
        /// </summary>
        public virtual bool IsSouth { get; set; }

        /// <summary>
        ///   True if the transform is defined.  That doesn't really mean it accurately represents the named
        ///   projection, but rather it won't throw a null exception during transformation for the lack of
        ///   a transform definition.
        /// </summary>
        public virtual bool IsValid { get; }

        /// <summary>
        ///   The zero point in geographic terms
        /// </summary>
        public virtual double? LatitudeOfOrigin { get; set; }

        /// <summary>
        /// The longitude of center for this coordinate system
        /// </summary>
        public virtual double? LongitudeOfCenter { get; set; }

        /// <summary>
        ///   Gets or sets the M.
        /// </summary>
        /// <value>
        ///   The M.
        /// </value>
        public virtual double? M { get; set; }

        /// <summary>
        ///   Gets or sets the name of this projection information
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        ///   A boolean that indicates whether to use the /usr/share/proj/proj_def.dat defaults file (proj4 parameter "no_defs").
        /// </summary>
        public virtual bool NoDefs { get; set; }

        /// <summary>
        ///   Gets or sets a boolean for the over-ranging flag
        /// </summary>
        public virtual bool Over { get; set; }

        /// <summary>
        ///   The scale factor for this coordinate system
        /// </summary>
        public virtual double ScaleFactor { get; set; }

        /// <summary>
        ///   The line of latitude where the scale information is preserved.
        /// </summary>
        public virtual double? StandardParallel1 { get; set; }

        /// <summary>
        /// The standard parallel 2.
        /// </summary>
        public virtual double? StandardParallel2 { get; set; }

        /// <summary>
        ///   The unit being used for measurements.
        /// </summary>
        public virtual LinearUnit Unit { get; set; }

        /// <summary>
        ///   Gets or sets the w.
        /// </summary>
        /// <value>
        ///   The w.
        /// </value>
        public virtual double? W { get; set; }

        /// <summary>
        ///   Gets or sets the integer zone parameter if it is specified.
        /// </summary>
        public virtual int? Zone { get; set; }


        // ReSharper disable InconsistentNaming
        /// <summary>
        ///   Gets or sets the alpha/ azimuth.
        /// </summary>
        /// <value>
        ///   ? Used with Oblique Mercator and possibly a few others. For our purposes this is exactly the same as azimuth
        /// </value>
        public virtual double? alpha { get; set; }

        /// <summary>
        ///   Gets or sets the BNS.
        /// </summary>
        /// <value>
        ///   The BNS.
        /// </value>
        public virtual int? bns { get; set; }

        /// <summary>
        ///   Gets or sets the czech.
        /// </summary>
        /// <value>
        ///   The czech.
        /// </value>
        public virtual int? czech { get; set; }

        /// <summary>
        ///   Gets or sets the guam.
        /// </summary>
        /// <value>
        ///   The guam.
        /// </value>
        public virtual bool? guam { get; set; }

        /// <summary>
        ///   Gets or sets the h.
        /// </summary>
        /// <value>
        ///   The h.
        /// </value>
        public virtual double? h { get; set; }

        /// <summary>
        ///   Gets or sets the lat_ts.
        /// </summary>
        /// <value>
        ///   Latitude of true scale.
        /// </value>
        public virtual double? lat_ts { get; set; }

        /// <summary>
        ///   Gets or sets the lon_1.
        /// </summary>
        /// <value>
        ///   The lon_1.
        /// </value>
        public virtual double? lon_1 { get; set; }

        /// <summary>
        ///   Gets or sets the lon_2.
        /// </summary>
        /// <value>
        ///   The lon_2.
        /// </value>
        public virtual double? lon_2 { get; set; }

        /// <summary>
        ///   Gets or sets the lonc.
        /// </summary>
        /// <value>
        ///   The lonc.
        /// </value>
        public virtual double? lonc { get; set; }

        /// <summary>
        ///   Gets or sets the m. Named mGeneral to prevent CLS conflicts.
        /// </summary>
        /// <value>
        ///   The m.
        /// </value>
        public virtual double? mGeneral { get; set; }

        /// <summary>
        ///   Gets or sets the n.
        /// </summary>
        /// <value>
        ///   The n.
        /// </value>
        public virtual double? n { get; set; }

        /// <summary>
        ///   Gets or sets the no_rot.
        /// </summary>
        /// <value>
        ///   The no_rot. Seems to be used as a boolean.
        /// </value>
        public virtual int? no_rot { get; set; }

        /// <summary>
        ///   Gets or sets the no_uoff.
        /// </summary>
        /// <value>
        ///   The no_uoff. Seems to be used as a boolean.
        /// </value>
        public virtual int? no_uoff { get; set; }

        /// <summary>
        ///   Gets or sets the rot_conv.
        /// </summary>
        /// <value>
        ///   The rot_conv. Seems to be used as a boolean.
        /// </value>
        public virtual int? rot_conv { get; set; }

        /// <summary>
        ///   Gets or sets the to_meter.
        /// </summary>
        /// <value>
        ///   Multiplier to convert map units to 1.0m
        /// </value>
        public virtual double? to_meter { get; set; }

        /// <summary>
        /// 从wkt导入
        /// </summary>
        /// <param name="wkt"></param>
        public abstract void ImportFromWkt(string wkt);
        /// <summary>
        /// 导出成wkt
        /// </summary>
        /// <returns></returns>
        public abstract string ExportToWkt();
        /// <summary>
        /// 从ESRI字符串导入
        /// </summary>
        /// <param name="wkt"></param>
        public abstract void ImportFromESRI(string wkt);
        /// <summary>
        /// 导入proj4
        /// </summary>
        /// <param name="proj4"></param>
        public abstract void ImportFromProj4(string proj4);
        /// <summary>
        /// 导出为Proj4
        /// </summary>
        /// <returns></returns>
        public abstract string ExportToProj4();
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                IsDisposed = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ProjectionInfo()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
