using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 双精度的点
    /// </summary>
    [Serializable]
    public struct PointD : IEquatable<PointD>
    {
        /// <summary>
        /// 新建一个<see cref='PointD'/> 实例
        /// </summary>
        public static readonly PointD Empty;
        private double x; // Do not rename (binary serialization)
        private double y; // Do not rename (binary serialization)

        /// <summary>
        /// 初始化 <see cref='PointD'/> 实例
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public PointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 初始化 <see cref='PointD'/> 实例
        /// </summary>
        /// <param name="vector">二维向量</param>
        public PointD(Vector2 vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        /// <summary>
        /// 从<see cref="PointD"/>创建一个<see cref="System.Numerics.Vector2"/>实例
        /// </summary>
        public Vector2 ToVector2() => new Vector2((float)x, (float)y);

        /// <summary>
        /// 计算是否为空
        /// </summary>
        [Browsable(false)]
        public readonly bool IsEmpty => x == 0f && y == 0f;

        /// <summary>
        /// x坐标
        /// </summary>
        public double X
        {
            readonly get => x;
            set => x = value;
        }

        /// <summary>
        /// y坐标
        /// </summary>
        public double Y
        {
            readonly get => y;
            set => y = value;
        }

        /// <summary>
        /// 将指定的<see cref="PointD"/>转换为<see cref="System.Numerics.Vector2"/>.
        /// </summary>
        public static explicit operator Vector2(PointD point) => point.ToVector2();

        /// <summary>
        /// 将指定的 <see cref="System.Numerics.Vector2"/>转换为<see cref="PointD"/>.
        /// </summary>
        public static explicit operator PointD(Vector2 vector) => new PointD(vector);

        /// <summary>
        /// 平移<see cref='PointD'/>至指定的<see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD operator +(PointD pt, Size sz) => Add(pt, sz);

        /// <summary>
        /// 平移<see cref='PointD'/>至相反的<see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD operator -(PointD pt, Size sz) => Subtract(pt, sz);

        /// <summary>
        /// 平移<see cref='PointD'/>至指定的<see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD operator +(PointD pt, SizeF sz) => Add(pt, sz);

        /// <summary>
        /// 平移<see cref='PointD'/> 至相反的<see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD operator -(PointD pt, SizeF sz) => Subtract(pt, sz);

        /// <summary>
        /// 计算两个点是否相等
        /// </summary>
        /// <param name="left">点1</param>
        /// <param name="right">点2</param>
        /// <returns>是否相等</returns>
        public static bool operator ==(PointD left, PointD right) => left.X == right.X && left.Y == right.Y;

        /// <summary>
        /// 计算两个点是否不等
        /// </summary>
        /// <param name="left">点1</param>
        /// <param name="right">点2</param>
        /// <returns>是否不等</returns>
        public static bool operator !=(PointD left, PointD right) => !(left == right);

        /// <summary>
        /// 平移<see cref='PointD'/>至指定的<see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD Add(PointD pt, Size sz) => new PointD(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// 平移<see cref='PointD'/>至相反的<see cref='System.Drawing.Size'/> .
        /// </summary>
        public static PointD Subtract(PointD pt, Size sz) => new PointD(pt.X - sz.Width, pt.Y - sz.Height);

        /// <summary>
        /// 平移<see cref='PointD'/>至指定的<see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD Add(PointD pt, SizeF sz) => new PointD(pt.X + sz.Width, pt.Y + sz.Height);

        /// <summary>
        /// 平移<see cref='PointD'/>相反的<see cref='System.Drawing.SizeF'/> .
        /// </summary>
        public static PointD Subtract(PointD pt, SizeF sz) => new PointD(pt.X - sz.Width, pt.Y - sz.Height);
        /// <inheritdoc/>
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is PointD && Equals((PointD)obj);
        /// <inheritdoc/>
        public readonly bool Equals(PointD other) => this == other;
        /// <inheritdoc/>
        public override readonly int GetHashCode() => HashCode.Combine(X.GetHashCode(), Y.GetHashCode());
        /// <inheritdoc/>
        public override readonly string ToString() => $"{{X={x}, Y={y}}}";
    }
}
