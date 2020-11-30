using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    public static class ExtentExtensions
    {
        /// <summary>
        /// 转几何体
        /// </summary>
        /// <param name="extent"></param>
        /// <returns></returns>
        public static IPolygon ToPolygon(this IExtent extent)
        {
            IPolygon polygon = null;
            if (extent != null)
            {
                var geometryFactory = DataFactory.Default.GeometryFactory;
                ICoordinate[] coordinates = { new Coordinate(extent.MinX, extent.MinY),
                    new Coordinate(extent.MinX, extent.MaxY),
                    new Coordinate(extent.MaxX, extent.MaxY),
                    new Coordinate(extent.MaxX, extent.MinY),
                    new Coordinate(extent.MinX, extent.MinY)
                };
                var shell = geometryFactory.GetLinearRing(coordinates);
                polygon = DataFactory.Default.GeometryFactory.GetPolygon(shell);
            }
            return polygon;
        }
    }
}
