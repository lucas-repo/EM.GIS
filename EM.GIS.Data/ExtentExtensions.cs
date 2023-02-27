using EM.GIS.Geometries;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 范围扩展类
    /// </summary>
    public static class ExtentExtensions
    {
        /// <summary>
        /// 扩展矩形
        /// </summary>
        /// <param name="rect0">矩形0</param>
        /// <param name="rect1">矩形1</param>
        /// <returns>扩展后的矩形</returns>
        public static RectangleF ExpandToInclude(this RectangleF rect0, RectangleF rect1)
        {
            RectangleF ret;
            if (rect0.IsEmpty)
            {
                ret = rect1;
            }
            else
            {
                if (rect1.IsEmpty)
                {
                    ret = rect0;
                }
                else
                {
                    ret = RectangleF.FromLTRB(Math.Min(rect0.Left, rect1.Left), Math.Min(rect0.Top, rect1.Top), Math.Min(rect0.Right, rect1.Right), Math.Min(rect0.Bottom, rect1.Bottom));
                }
            }
            return ret;
        }
        /// <summary>
        /// 扩展矩形
        /// </summary>
        /// <param name="rect0">矩形0</param>
        /// <param name="rect1">矩形1</param>
        /// <returns>扩展后的矩形</returns>
        public static Rectangle ExpandToInclude(this Rectangle rect0, Rectangle rect1)
        {
            Rectangle ret;
            if (rect0.IsEmpty)
            {
                ret = rect1;
            }
            else
            {
                if (rect1.IsEmpty)
                {
                    ret = rect0;
                }
                else
                {
                    ret = Rectangle.FromLTRB(Math.Min(rect0.Left, rect1.Left), Math.Min(rect0.Top, rect1.Top), Math.Min(rect0.Right, rect1.Right), Math.Min(rect0.Bottom, rect1.Bottom));
                }
            }
            return ret;
        }
        /// <summary>
        /// 将范围转几何体
        /// </summary>
        /// <param name="geometryFactory">几何工厂</param>
        /// <param name="extent">范围</param>
        /// <returns>几何体</returns>
        public static IGeometry ToPolygon(this IGeometryFactory geometryFactory, IExtent extent)
        {
            ICoordinate[] coordinates = { new Coordinate(extent.MinX, extent.MinY),
                new Coordinate(extent.MinX, extent.MaxY),
                new Coordinate(extent.MaxX, extent.MaxY),
                new Coordinate(extent.MaxX, extent.MinY),
                new Coordinate(extent.MinX, extent.MinY)
            };
            var polygon = geometryFactory.GetPolygon(coordinates);
            return polygon;
        }

        /// <summary>
        /// 根据当前长宽比，重设范围的长宽比
        /// </summary>
        /// <param name="extent">范围</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        public static void ResetAspectRatio(IExtent extent, int width, int height)
        {
            // Aspect Ratio Handling
            if (extent == null || extent.IsEmpty()) return;

            // It isn't exactly an exception, but rather just an indication not to do anything here.
            if (height == 0 || width == 0) return;

            double controlAspect = (double)width / height;
            double envelopeAspect = extent.Width / extent.Height;
            var center = extent.Center;

            if (controlAspect > envelopeAspect)
            {
                // The Control is proportionally wider than the envelope to display.
                // If the envelope is proportionately wider than the control, "reveal" more width without
                // changing height If the envelope is proportionately taller than the control,
                // "hide" width without changing height
                extent.SetCenter(center, extent.Height * controlAspect, extent.Height);
            }
            else
            {
                // The control is proportionally taller than the content is
                // If the envelope is proportionately wider than the control,
                // "hide" the extra height without changing width
                // If the envelope is proportionately taller than the control, "reveal" more height without changing width
                extent.SetCenter(center, extent.Width, extent.Width / controlAspect);
            }
        }
        /// <summary>
        /// 将<see cref="BruTile.Extent"/>转为<see cref="IExtent"/>
        /// </summary>
        /// <param name="extent"><see cref="BruTile.Extent"/></param>
        /// <returns><see cref="IExtent"/></returns>
        public static IExtent ToExtent(this BruTile.Extent extent)
        {
            return new Extent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }
        /// <summary>
        /// 将<see cref="IExtent"/>转为<see cref="BruTile.Extent"/>
        /// </summary>
        /// <param name="extent"><see cref="IExtent"/></param>
        /// <returns><see cref="BruTile.Extent"/></returns>
        public static  BruTile.Extent ToExtent(this IExtent extent)
        {
            return new BruTile.Extent(extent.MinX, extent.MinY, extent.MaxX, extent.MaxY);
        }
        /// <summary>
        /// 将范围转为面
        /// </summary>
        /// <param name="extent">范围</param>
        /// <returns>面</returns>
        public static IGeometry? ToPolygon(this BruTile.Extent extent)
        {
            IGeometry? ret = null;
            if (extent == null)
            {
                return ret;
            }
            var geometryFactory = IocManager.Default.GetService<IGeometryFactory>();
            if (geometryFactory != null)
            {
                List<ICoordinate> coordinates =new List<ICoordinate>() { 
                    new Coordinate(extent.MinX,extent.MinY),
                    new Coordinate(extent.MaxX,extent.MinY),
                    new Coordinate(extent.MaxX,extent.MaxY),
                    new Coordinate(extent.MinX,extent.MaxY)
                };
                coordinates.Add(coordinates[0]);
                ret = geometryFactory.GetPolygon(coordinates);
            }
            return ret;
        }
    }
}
