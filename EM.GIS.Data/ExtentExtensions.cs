using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
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
    }
}
