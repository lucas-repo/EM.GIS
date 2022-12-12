using System;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 坐标接口
    /// </summary>
    public interface ICoordinate : ICloneable, IComparable, IComparable<ICoordinate>, IEquatable<ICoordinate>
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
        /// 维度
        /// </summary>
        int Dimension { get; }
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
        /// 是否为空（x或y为double.NAN）
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// 判断与指定坐标是否近似相等
        /// </summary>
        /// <param name="c">坐标</param>
        /// <param name="tolerance">近似值</param>
        /// <returns>相等为true反之false</returns>
        bool Equals2D(ICoordinate c, double tolerance = 0);
        /// <summary>
        /// 判断与指定坐标是否近似相等
        /// </summary>
        /// <param name="c">坐标</param>
        /// <param name="tolerance">近似值</param>
        /// <returns>相等为true反之false</returns>
        bool Equals3D(ICoordinate c, double tolerance = 0);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="c">坐标</param>
        /// <returns>距离</returns>
        double Distance(ICoordinate c);
        /// <summary>
        /// 计算距离
        /// </summary>
        /// <param name="c">坐标</param>
        /// <returns>距离</returns>
        double Distance3D(ICoordinate c);
        /// <summary>
        /// 转为数组
        /// </summary>
        /// <param name="dimension">维度</param>
        /// <returns>数组</returns>
        double[] ToDoubleArray(int dimension = 2);
    }
}