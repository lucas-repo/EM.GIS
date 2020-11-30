using EM.GIS.Geometries;
using EM.GIS.Projection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EM.GIS.Data
{
    public static class ProjectionExtensions
    {
        /// <summary>
        /// 转换坐标
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static ICoordinate TransformCoordinate(this ITransformation transformation, ICoordinate coordinate)
        {
            ICoordinate destCoord = coordinate;
            if (transformation == null || coordinate == null || coordinate.IsEmpty())
            {
                return destCoord;
            }
            double[] array;
            if (!double.IsNaN(coordinate.Z))
            {
                array = new double[] { coordinate.X, coordinate.Y, coordinate.Z };
            }
            else
            {
                array = new double[] { coordinate.X, coordinate.Y };
            }
            transformation.TransformPoint(array);
            if (!double.IsNaN(coordinate.Z))
            {
                destCoord = new Coordinate(array[0], array[1], array[2]);
            }
            else
            {
                destCoord = new Coordinate(array[0], array[1]);
            }
            return destCoord;
        }
        /// <summary>
        /// 转换多个坐标
        /// </summary>
        /// <param name="transformation"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static List<ICoordinate> TransformCoordinates(this ITransformation transformation, IEnumerable<ICoordinate> coordinates)
        {
            List<ICoordinate> destCoordinates = new List<ICoordinate>();
            if (transformation == null || coordinates == null)
            {
                return destCoordinates;
            }
            var count = coordinates.Count();
            if (count > 0)
            {
                double[] xs = new double[count];
                double[] ys = new double[count];
                double[] zs = new double[count];
                for (int i = 0; i < count; i++)
                {
                    var coord = coordinates.ElementAt(i);
                    xs[i] = coord.X;
                    ys[i] = coord.Y;
                    if (!double.IsNaN(coord.Z))
                    {
                        zs[i] = coord.Z;
                    }
                }
                transformation.TransformPoints(count, xs, ys, zs);
                for (int i = 0; i < count; i++)
                {
                    var coord = new Coordinate(xs[i], ys[i], zs[i]);
                    destCoordinates.Add(coord);
                }
            }
            return destCoordinates;
        }
    }
}
