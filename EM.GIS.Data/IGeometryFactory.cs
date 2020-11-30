using EM.GIS.Geometries;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace EM.GIS.Data
{
    /// <summary>
    /// 几何工厂
    /// </summary>
    [InheritedExport]
    public interface IGeometryFactory
    {
        /// <summary>
        /// 从wkt获取几何体
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        IGeometry GetGeometryFromWkt(string wkt);
        /// <summary>
        /// 创建点
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        IPoint GetPoint(ICoordinate coordinate);
        /// <summary>
        /// 创建线
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        ILineString GetLineString(IEnumerable<ICoordinate> coordinates);
        /// <summary>
        /// 创建环
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        ILinearRing GetLinearRing(IEnumerable<ICoordinate> coordinates);
        /// <summary>
        /// 创建面
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="holes"></param>
        /// <returns></returns>
        IPolygon GetPolygon(ILinearRing shell, IEnumerable<ILinearRing> holes = null);
    }
}