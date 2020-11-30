using System;

namespace EM.GIS.Geometries
{
    /// <summary>
    /// 范围接口
    /// </summary>
    public interface IExtent:ICloneable
    {
        /// <summary>
        /// 最小X
        /// </summary>
        double MinX { get; set; }
        /// <summary>
        /// 最大X
        /// </summary>
        double MaxX { get; set; }

        /// <summary>
        /// 最小Y
        /// </summary>
        double MinY { get; set; }
        /// <summary>
        /// 最大Y
        /// </summary>
        double MaxY { get; set; }
        /// <summary>
        /// 最小M
        /// </summary>
        double MinM { get; set; }
        /// <summary>
        /// 最大M
        /// </summary>
        double MaxM { get; set; }
        /// <summary>
        /// 最小Z
        /// </summary>
        double MinZ { get; set; }
        /// <summary>
        /// 最大Z
        /// </summary>
        double MaxZ { get; set; }
        /// <summary>
        /// 有M值
        /// </summary>
        bool HasM { get; }
        /// <summary>
        /// 有Z值
        /// </summary>
        bool HasZ { get; }
        /// <summary>
        /// X值
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Y值
        /// </summary>
        double Y { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        double Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        double Height { get; set; }
        /// <summary>
        /// 中心点
        /// </summary>
        ICoordinate Center { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty();
        /// <summary>
        /// 是否包含点
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool Contains(ICoordinate c);
        /// <summary>
        /// 是否包含范围
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        bool Contains(IExtent ext);
        /// <summary>
        /// 是否包含范围
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        bool Contains(double minX, double maxX, double minY, double maxY);
        /// <summary>
        /// 是否包含于范围
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        bool Within(IExtent ext);
        /// <summary>
        /// 是否包含于范围
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        bool Within(double minX, double maxX, double minY, double maxY);
        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        bool Intersects(IExtent ext);
        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <returns></returns>
        bool Intersects(double minX, double maxX, double minY, double maxY);
        /// <summary>
        /// 设置中心点
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetCenter(double centerX, double centerY, double width, double height);
        /// <summary>
        /// 设置中心点
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void SetCenter(ICoordinate center, double width, double height);
        /// <summary>
        /// 设置中心点
        /// </summary>
        /// <param name="center"></param>
        void SetCenter(ICoordinate center);
        /// <summary>
        /// 扩展范围
        /// </summary>
        /// <param name="padX"></param>
        /// <param name="padY"></param>
        void ExpandBy(double padX, double padY);
        /// <summary>
        /// 扩展范围
        /// </summary>
        /// <param name="padding"></param>
        void ExpandBy(double padding);
        /// <summary>
        /// 扩展范围
        /// </summary>
        /// <param name="ext"></param>
        void ExpandToInclude(IExtent ext);
    }
}