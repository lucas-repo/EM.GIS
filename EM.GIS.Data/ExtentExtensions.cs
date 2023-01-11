﻿using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 范围扩展类
    /// </summary>
    public static class ExtentExtensions
    {
        /// <summary>
        /// 将范围转几何体
        /// </summary>
        /// <param name="geometryFactory">几何工厂</param>
        /// <param name="extent">范围</param>
        /// <returns>几何体</returns>
        public static IGeometry ToPolygon(this IGeometryFactory geometryFactory, IExtent extent)
        {
            IGeometry polygon = null;
            if (geometryFactory!=null&& extent != null)
            {
                ICoordinate[] coordinates = { new Coordinate(extent.MinX, extent.MinY),
                    new Coordinate(extent.MinX, extent.MaxY),
                    new Coordinate(extent.MaxX, extent.MaxY),
                    new Coordinate(extent.MaxX, extent.MinY),
                    new Coordinate(extent.MinX, extent.MinY)
                };
                polygon = geometryFactory.GetPolygon(coordinates);
            }
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
    }
}
