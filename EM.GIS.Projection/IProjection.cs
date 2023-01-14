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
        /// 重投影多个坐标
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="extent">范围</param>
        /// <returns>范围</returns>
        void ReProject(IProjection destProjection, IExtent extent);
        /// <summary>
        /// 重投影几何体
        /// </summary>
        /// <param name="destProjection">目标投影</param>
        /// <param name="geometry">几何体</param>
        void ReProject(IProjection destProjection, IGeometry geometry);
    }
}
