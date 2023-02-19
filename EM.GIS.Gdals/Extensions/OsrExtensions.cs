using EM.GIS.Geometries;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class OsrExtensions
    {
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="coordinateTransformation">坐标转换器</param>
        /// <param name="coord">坐标</param>
        public static void Transform(this CoordinateTransformation coordinateTransformation, ICoordinate coord)
        {
            if (coordinateTransformation != null && coord != null)
            {
                var array = coord.ToDoubleArray(coord.Dimension);
                coordinateTransformation.TransformPoint(array);
                for (int i = 0; i < coord.Dimension; i++)
                {
                    coord[i] = array[i];
                }
            }
        }

        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="coordinateTransformation">坐标转换器</param>
        /// <param name="geometry">几何体</param>
        public static void Transform(this CoordinateTransformation coordinateTransformation, OSGeo.OGR.Geometry geometry)
        {
            if (coordinateTransformation == null || geometry == null)
            {
                return;
            }
            var geometryCount= geometry.GetGeometryCount();
            if (geometryCount > 0)
            {
                for (int i = 0; i < geometryCount; i++)
                {
                    var geoItem = geometry.GetGeometryRef(i);
                    coordinateTransformation.Transform(geoItem);
                }
            }
            else
            {
                var pointCount = geometry.GetPointCount();
                var dimension = geometry.GetCoordinateDimension();
                if (dimension < 2)
                {
                    return;
                }

                double[] xs = new double[pointCount];
                double[] ys = new double[pointCount];
                double[] zs = new double[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    double[] argout = new double[dimension];
                    geometry.GetPoint(i, argout);
                    xs[i] = argout[0];
                    ys[i] = argout[1];
                    if (dimension > 2)
                    {
                        zs[i] = argout[2];
                    }
                }
                coordinateTransformation.TransformPoints(pointCount, xs, ys, zs);
                if (dimension > 2)
                {
                    for (int i = 0; i < pointCount; i++)
                    {
                        geometry.SetPoint(i, xs[i], ys[i], zs[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < pointCount; i++)
                    {
                        geometry.SetPoint_2D(i, xs[i], ys[i]);
                    }
                }
            }
        }
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="coordinateTransformation">坐标转换器</param>
        /// <param name="geometry">几何体</param>
        public static void Transform(this CoordinateTransformation coordinateTransformation, IGeometry geometry)
        {
            if (coordinateTransformation == null || !(geometry is Geometry destGeometry))
            {
                return;
            }
            coordinateTransformation.Transform(destGeometry.OgrGeometry);
        }
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="coordinateTransformation">坐标转换器</param>
        /// <param name="coords">坐标</param>
        /// <param name="count">转换个数</param>
        public static void Transform(this CoordinateTransformation coordinateTransformation, IList<ICoordinate> coords, int count)
        {
            if (coordinateTransformation != null && coords != null)
            {
                var firstCoord = coords.FirstOrDefault();
                if (firstCoord != null)
                {
                    var dimension = firstCoord.Dimension;
                    if (dimension >= 2)
                    {
                        double[] xs = new double[count];
                        double[] ys = new double[count];
                        double[] zs = new double[count];
                        for (int i = 0; i < count; i++)
                        {
                            var coord = coords[i];
                            xs[i] = coord.X;
                            ys[i] = coord.Y;
                            if (dimension > 2)
                            {
                                zs[i] = coord.Z;
                            }
                        }
                        coordinateTransformation.TransformPoints(count, xs, ys, zs);
                        for (int i = 0; i < count; i++)
                        {
                            var coord = coords[i];
                            coord.X = xs[i];
                            coord.Y = ys[i];
                            if (dimension > 2)
                            {
                                coord.Z = zs[i];
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 范围坐标转换
        /// </summary>
        /// <param name="coordinateTransformation">坐标转换器</param>
        /// <param name="extent">范围</param>
        public static void Transform(this CoordinateTransformation coordinateTransformation, IExtent extent)
        {
            if (coordinateTransformation != null && extent != null)
            {
                double[] xs = { extent.MinX, extent.MaxX };
                double[] ys = { extent.MinY, extent.MaxY };
                double[] zs = { 0, 0 };
                coordinateTransformation.TransformPoints(2, xs, ys, zs);
                double minX, minY, maxX, maxY;
                if (xs[0] > xs[1])
                {
                    minX = xs[1];
                    maxX = xs[0];
                }
                else
                {
                    minX = xs[0];
                    maxX = xs[1];
                }
                if (ys[0] > ys[1])
                {
                    minY = ys[1];
                    maxY = ys[0];
                }
                else
                {
                    minY = ys[0];
                    maxY = ys[1];
                }
                extent.MinX=minX;
                extent.MinY = minY;
                extent.MaxX = maxX;
                extent.MaxY  = maxY;
            }
        }
    }
}
