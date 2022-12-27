using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 双精度的矩形
    /// </summary>
    [Serializable]
    public struct RectangleD : IEquatable<RectangleD>
    {
        /// <summary>
        /// 空矩形
        /// </summary>
        public static readonly RectangleD Empty;

        private double x;
        private double y;
        private double width;
        private double height;
        /// <summary>
        /// 实例化<seealso cref="RectangleD"/>
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public RectangleD(double x, double y, double width, double height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 创建<seealso cref="RectangleD"/>
        /// </summary>
        /// <param name="left">左边x坐标</param>
        /// <param name="top">上边y坐标</param>
        /// <param name="right">右边x坐标</param>
        /// <param name="bottom">下边y坐标</param>
        /// <returns></returns>
        public static RectangleD FromLTRB(double left, double top, double right, double bottom) =>
            new RectangleD(left, top, right - left, bottom - top);

        /// <summary>
        /// 左边x坐标
        /// </summary>
        public double X
        {
            get => x;
            set => x = value;
        }

        /// <summary>
        /// 上边y坐标
        /// </summary>
        public double Y
        {
            get => y;
            set => y = value;
        }

        /// <summary>
        /// 宽度
        /// </summary>
        public double Width
        {
            get => width;
            set => width = value;
        }

        /// <summary>
        /// 高度
        /// </summary>
        public double Height
        {
            get => height;
            set => height = value;
        }

        /// <summary>
        /// 左边x坐标
        /// </summary>
        [Browsable(false)]
        public double Left => X;

        /// <summary>
        /// 上边y坐标
        /// </summary>
        [Browsable(false)]
        public double Top => Y;

        /// <summary>
        /// 右边x坐标
        /// </summary>
        [Browsable(false)]
        public double Right => X + Width;

        /// <summary>
        /// 下边y坐标
        /// </summary>
        [Browsable(false)]
        public double Bottom => Y + Height;

        /// <summary>
        /// 宽度或高度小于等于0则为空
        /// </summary>
        [Browsable(false)]
        public bool IsEmpty => (Width <= 0) || (Height <= 0);

        /// <summary>
        /// Tests whether <paramref name="obj"/> is a <see cref='System.Drawing.RectangleF'/> with the same location and
        /// size of this <see cref='System.Drawing.RectangleF'/>.
        /// </summary>
        public override bool Equals(object obj) => obj is RectangleF && Equals((RectangleF)obj);
        /// <inheritdoc/>
        public bool Equals(RectangleD other) => this == other;
        /// <summary>
        /// 计算两个范围是否相等
        /// </summary>
        /// <param name="left">范围1</param>
        /// <param name="right">范围2</param>
        /// <returns>相等为true反之false</returns>
        public static bool operator ==(RectangleD left, RectangleD right) =>
            left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        /// <summary>
        /// 计算两个范围是否不等
        /// </summary>
        /// <param name="left">范围1</param>
        /// <param name="right">范围2</param>
        /// <returns>不等为true反之false</returns>
        public static bool operator !=(RectangleD left, RectangleD right) => !(left == right);

        /// <summary>
        /// 是否包含指定坐标
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <returns>包含则为true反之false</returns>
        public bool Contains(double x, double y) => X <= x && x < X + Width && Y <= y && y < Y + Height;

        /// <summary>
        /// 是否包含指定坐标
        /// </summary>
        /// <param name="pt">坐标</param>
        /// <returns>包含则为true反之false</returns>
        public bool Contains(PointF pt) => Contains(pt.X, pt.Y);

        /// <summary>
        /// 是否包含指定范围
        /// </summary>
        /// <param name="rect">范围</param>
        /// <returns>包含则为true反之false</returns>
        public bool Contains(RectangleD rect) =>
            (X <= rect.X) && (rect.X + rect.Width <= X + Width) && (Y <= rect.Y) && (rect.Y + rect.Height <= Y + Height);
        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);

        /// <summary>
        /// 扩展
        /// </summary>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        public void Inflate(double x, double y)
        {
            X -= x;
            Y -= y;
            Width += 2 * x;
            Height += 2 * y;
        }

        /// <summary>
        /// 扩展
        /// </summary>
        /// <param name="size">大小</param>
        public void Inflate(SizeF size) => Inflate(size.Width, size.Height);

        /// <summary>
        /// 扩展范围
        /// </summary>
        /// <param name="rect">范围</param>
        /// <param name="x">x值</param>
        /// <param name="y">y值</param>
        /// <returns>范围</returns>
        public static RectangleD Inflate(RectangleD rect, double x, double y)
        {
            RectangleD r = rect;
            r.Inflate(x, y);
            return r;
        }

        /// <summary>
        /// 判断是否相交
        /// </summary>
        /// <param name="rect">范围</param>
        public void Intersect(RectangleD rect)
        {
            var result = Intersect(rect, this);

            X = result.X;
            Y = result.Y;
            Width = result.Width;
            Height = result.Height;
        }
        /// <summary>
        /// 相交
        /// </summary>
        /// <param name="a">矩形1</param>
        /// <param name="b">矩形2</param>
        /// <returns></returns>
        public static RectangleD Intersect(RectangleD a, RectangleD b)
        {
            var x1 = Math.Max(a.X, b.X);
            var x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            var y1 = Math.Max(a.Y, b.Y);
            var y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1 && y2 >= y1)
            {
                return new RectangleD(x1, y1, x2 - x1, y2 - y1);
            }

            return Empty;
        }

        /// <summary>
        /// 判断是否相交
        /// </summary>
        /// <param name="rect">矩形</param>
        /// <returns>相交为true</returns>
        public readonly bool IntersectsWith(RectangleD rect) =>
            (rect.X < X + Width) && (X < rect.X + rect.Width) && (rect.Y < Y + Height) && (Y < rect.Y + rect.Height);

        /// <summary>
        /// 合并矩形
        /// </summary>
        /// <param name="a">矩形1</param>
        /// <param name="b">矩形2</param>
        /// <returns></returns>
        public static RectangleD Union(RectangleD a, RectangleD b)
        {
            var x1 = Math.Min(a.X, b.X);
            var x2 = Math.Max(a.X + a.Width, b.X + b.Width);
            var y1 = Math.Min(a.Y, b.Y);
            var y2 = Math.Max(a.Y + a.Height, b.Y + b.Height);

            return new RectangleD(x1, y1, x2 - x1, y2 - y1);
        }

        /// <summary>
        /// 偏移
        /// </summary>
        /// <param name="pos">向量坐标</param>
        public void Offset(PointF pos) => Offset(pos.X, pos.Y);

        /// <summary>
        /// 偏移
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public void Offset(double x, double y)
        {
            X += x;
            Y += y;
        }

        /// <summary>
        /// 将<seealso cref="RectangleF"/>转换成<seealso cref="RectangleD"/>
        /// </summary>
        /// <param name="r">矩形</param>
        public static implicit operator RectangleD(RectangleF r) => new RectangleD(r.X, r.Y, r.Width, r.Height);
        /// <inheritdoc/>
        public override readonly string ToString() => $"{{X={X},Y={Y},Width={Width},Height={Height}}}";
    }
}
