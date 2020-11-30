using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Data
{
    /// <summary>
    /// 要素接口
    /// </summary>
    public interface IFeature:ICloneable,IDisposable,IGetField
    {
        /// <summary>
        /// 要素id
        /// </summary>
        long FId { get; set; }
        /// <summary>
        /// 几何体
        /// </summary>
        IGeometry Geometry { get; set; }
    }
}