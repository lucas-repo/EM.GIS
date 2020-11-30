using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 几何接口
    /// </summary>
    public interface IGeometry:ICloneable
    {
        /// <summary>
        /// 几何类型
        /// </summary>
        GeometryType GeometryType { get; }
        /// <summary>
        /// 点个数
        /// </summary>
        int PointCount { get; }
        /// <summary>
        /// 获取第一个点
        /// </summary>
        ICoordinate Coord { get; }
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// 获取点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ICoordinate GetCoord(int index);
        /// <summary>
        /// 获取所有点
        /// </summary>
        /// <returns></returns>
        List<ICoordinate> GetAllCoords();
        /// <summary>
        /// 设置点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="coordinate"></param>
        void SetCoord(int index, ICoordinate coordinate);
        /// <summary>
        /// 几何个数
        /// </summary>
        int GeometryCount { get; }
        /// <summary>
        /// 获取几何
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGeometry GetGeometry(int index);
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; }
        /// <summary>
        /// 转成Well-known Text
        /// </summary>
        /// <returns></returns>
        string ToWkt();

        /// <summary>
        /// 面积
        /// </summary>
        double Area { get; }
        /// <summary>
        /// 长度
        /// </summary>
        double Length { get; }
        
        #region 几何运算

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        bool Contains(IGeometry g);
        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        bool Intersects(IGeometry g);
        /// <summary>
        /// 求交集
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IGeometry Intersection(IGeometry other);
        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IGeometry Union(IGeometry other);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        double Distance(IGeometry g);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        double Distance(ICoordinate coord);
        #endregion
        
    }
}