using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 几何接口
    /// </summary>
    public interface IGeometry : ICloneable, IDisposable
    {
        /// <summary>
        /// 几何类型
        /// </summary>
        GeometryType GeometryType { get; }
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// 获取几何体个数
        /// </summary>
        int GeometryCount { get; }
        /// <summary>
        /// 获取指定的几何体
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>几何体</returns>
        IGeometry GetGeometry(int index);
        /// <summary>
        /// 获取点个数
        /// </summary>
        int CoordinateCount { get; }
        /// <summary>
        /// 获取指定点坐标
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>点坐标</returns>
        ICoordinate GetCoordinate(int index);

        /// <summary>
        /// 范围
        /// </summary>
        IExtent GetExtent();
        /// <summary>
        /// 转成Well-known Text
        /// </summary>
        /// <returns></returns>
        string ToWkt();

        /// <summary>
        /// 面积
        /// </summary>
        double Area();
        /// <summary>
        /// 长度
        /// </summary>
        double Length();

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