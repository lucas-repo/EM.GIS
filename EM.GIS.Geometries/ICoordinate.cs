using System;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 坐标接口
    /// </summary>
    public interface ICoordinate : ICloneable
    {
        /// <summary>
        /// 返回指定索引值
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double this[int index] { get; set; }
        /// <summary>
        /// 最大索引个数
        /// </summary>
        int MaxPossibleOrdinates { get; }
        /// <summary>
        /// 索引个数
        /// </summary>
        int NumOrdinates { get; }
        /// <summary>
        /// X值
        /// </summary>
        double X { get; set; }
        /// <summary>
        /// Y值
        /// </summary>
        double Y { get; set; }
        /// <summary>
        /// Z值
        /// </summary>
        double Z { get; set; }
        /// <summary>
        /// M值
        /// </summary>
        double M { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="c"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        bool Equals2D(Coordinate c, double tolerance = 0);
        /// <summary>
        /// 判断相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Equals3D(Coordinate other);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        double Distance(Coordinate c);
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        double Distance3D(Coordinate c);
        /// <summary>
        /// 转数组
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        double[] ToDoubleArray(int dimension = 2);
    }
}