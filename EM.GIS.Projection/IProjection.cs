using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 投影接口
    /// </summary>
    public interface IProjection
    {
        /// <summary>
        ///  认证机构（如EPSG）
        /// </summary>
        string Authority { get; set; }
        /// <summary>
        /// 投影编码
        /// </summary>
        string AuthorityCode { get; set; }
        /// <summary>
        /// 投影编码
        /// </summary>
        int? EPSG { get; }
        /// <summary>
        /// 是否为地理坐标系
        /// </summary>
        bool IsLatLon { get; }
        /// <summary>
        /// 地理坐标系信息
        /// </summary>
        GeographicInfo GeographicInfo { get; set; }
        /// <summary>
        /// 线性单位
        /// </summary>
        LinearUnit Unit { get; set; }
        /// <summary>
        /// 重投影坐标
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="coordinate">坐标</param>
        public abstract void ReProject(IProjection destProjection, ICoordinate coordinate);
        /// <summary>
        /// 重投影坐标
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="coordinates">坐标</param>
        void ReProject(IProjection destProjection, IList<ICoordinate> coordinates);
        /// <summary>
        /// 重投影范围
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="extent">范围</param>
        /// <returns>范围</returns>
        void ReProject(IProjection destProjection, IExtent extent);
        /// <summary>
        /// 重投影范围
        /// </summary>
        /// <param name="destProjectionEpsg">目标投影</param>
        /// <param name="extent">范围</param>
        /// <returns>范围</returns>
        void ReProject(int destProjectionEpsg, IExtent extent);
        /// <summary>
        /// 重投影几何体
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="geometry">几何体</param>
        void ReProject(IProjection destProjection, IGeometry geometry);
        /// <summary>
        /// 计算两点的距离（米）
        /// </summary>
        /// <param name="coord1">第一个坐标</param>
        /// <param name="coord2">第二个坐标</param>
        /// <returns>距离（米）</returns>
        double GetLengthOfMeters(ICoordinate coord1,ICoordinate coord2);
        /// <summary>
        /// 计算两点的距离（米）
        /// </summary>
        /// <param name="x1">第一个点x</param>
        /// <param name="y1">第一个点y</param>
        /// <param name="x2">第二个点x</param>
        /// <param name="y2">第二个点y</param>
        /// <returns>距离（米）</returns>
        double GetLengthOfMeters(double x1, double y1, double x2, double y2);
    }
}
