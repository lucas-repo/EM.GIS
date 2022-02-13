using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// 获取几何体的所有点个数
        /// </summary>
        /// <param name="geometry">几何体</param>
        /// <returns>点个数</returns>
        public static int GetTotalPointCount(this Geometry geometry)
        {
            int count = 0;
            if (geometry != null)
            {
                int geoCount = geometry.GetGeometryCount();//只有 wkbPolygon[25D]、wkbMultiPoint[25D]、wkbMultiLineString[25D]、wkbMultiPolygon[25D] 或 wkbGeometryCollection[25D] 类型的几何图形可以返回有效值。其他几何类型将静默返回 0。对于多边形，返回的数字是环数（外环 + 内环）。
                if (geoCount > 0)
                {
                    for (int i = 0; i < geoCount; i++)
                    {
                        var childGeo = geometry.GetGeometryRef(i);
                        count+= childGeo.GetTotalPointCount();
                    }
                }
                else
                {
                    count = geometry.GetPointCount();
                }
            }
            return count;
        }
        /// <summary>
        /// 添加点
        /// </summary>
        /// <param name="geometry">几何体</param>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        /// <param name="z">z</param>
        /// <param name="m">m</param>
        public static void AddPoint(this Geometry geometry, double x, double y, double z = double.NaN, double m = double.NaN)
        {
            if (geometry != null)
            {
                if (double.IsNaN(z))
                {
                    if (double.IsNaN(m))
                    {
                        geometry.AddPoint_2D(x, y);
                    }
                    else
                    {
                        geometry.AddPointM(x, y, m);
                    }
                }
                else
                {
                    if (double.IsNaN(m))
                    {
                        geometry.AddPoint(x, y, z);
                    }
                    else
                    {
                        geometry.AddPointZM(x, y, z, m);
                    }
                }
            }
        }
        /// <summary>
        /// 坐标转换
        /// </summary>
        /// <param name="geometry">几何体</param>
        /// <param name="transformCoordAction">坐标转换方法</param>
        public static void TransformCoord(this Geometry geometry, Action<double[]> transformCoordAction)
        {
            if (geometry==null||transformCoordAction==null)
            {
                return;
            }
            var geometryCount = geometry.GetGeometryCount();
            if (geometryCount==0)
            {
                int pointCount = geometry.GetPointCount();
                var dimension = geometry.GetCoordinateDimension();
                double[] coord = new double[dimension];
                for (int pointIndex = 0; pointIndex < pointCount; pointIndex++)
                {
                    switch (dimension)
                    {
                        case 2:
                            geometry.GetPoint_2D(pointIndex, coord);
                            transformCoordAction(coord);
                            geometry.SetPoint_2D(pointIndex, coord[0], coord[1]);
                            break;
                        case 3:
                            geometry.GetPoint(pointIndex, coord);
                            transformCoordAction(coord);
                            if (geometry.Is3D()==1)
                            {
                                geometry.SetPoint(pointIndex, coord[0], coord[1], coord[2]);
                            }
                            else if (geometry.IsMeasured()==1)
                            {
                                geometry.SetPointM(pointIndex, coord[0], coord[1], coord[2]);
                            }
                            break;
                        case 4:
                            geometry.GetPointZM(pointIndex, coord);
                            transformCoordAction(coord);
                            geometry.SetPointZM(pointIndex, coord[0], coord[1], coord[2], coord[3]);
                            break;
                    }
                }
            }
            else//只有 wkbPolygon[25D]、wkbMultiPoint[25D]、wkbMultiLineString[25D]、wkbMultiPolygon[25D] 或 wkbGeometryCollection[25D] 类型的几何图形可以返回有效值。其他几何类型将静默返回 0。对于多边形，返回的数字是环数（外环 + 内环）。
            {
                for (int geometryIndex = 0; geometryIndex < geometryCount; geometryIndex++)
                {
                    var childGeometry = geometry.GetGeometryRef(geometryIndex);
                    childGeometry.TransformCoord(transformCoordAction);
                }
            }
        }
       
        /// <summary>
        /// 设置点坐标（geometry必须为线或点类型,coord长度必须与几何体维度一致）
        /// </summary>
        /// <param name="geometry">几何体</param>
        /// <param name="pointIndex">点索引</param>
        /// <param name="coord">坐标</param>
        /// <returns>设置成功为true反之false</returns>
        public static bool SetPoint(this Geometry geometry, int pointIndex, double[] coord)
        {
            bool ret = false;
            if (geometry!=null&&pointIndex>=0&&pointIndex<geometry.GetPointCount()&&coord?.Length==geometry.GetCoordinateDimension())
            {
                switch (coord.Length)
                {
                    case 2:
                        geometry.SetPoint_2D(pointIndex, coord[0], coord[1]);
                        break;
                    case 3:
                        if (geometry.Is3D()==1)
                        {
                            geometry.SetPoint(pointIndex, coord[0], coord[1], coord[2]);
                        }
                        else if (geometry.IsMeasured()==1)
                        {
                            geometry.SetPointM(pointIndex, coord[0], coord[1], coord[2]);
                        }
                        break;
                    case 4:
                        geometry.SetPointZM(pointIndex, coord[0], coord[1], coord[2], coord[3]);
                        break;
                }
                ret=true;
            }
            return ret;
        }
        /// <summary>
        /// 计算几何体集合的质心
        /// </summary>
        /// <param name="geometries">几何体集合</param>
        /// <returns>质心</returns>
        public static Geometry GetCentroid(this IEnumerable<Geometry> geometries)
        {
            Geometry centroid = null;
            if (geometries!=null&&geometries.Count()>0)
            {
                var firstGeometry = geometries.First();
                var dimension = firstGeometry.GetCoordinateDimension();
                double[] destCoord = new double[dimension];
                foreach (var geometry in geometries)
                {
                    double[] coord = new double[dimension];
                    geometry.Centroid().GetPoint(0, coord);
                    for (int i = 0; i < dimension; i++)
                    {
                        destCoord[i] += coord[i];
                    }
                }
                for (int i = 0; i < dimension; i++)
                {
                    destCoord[i]/=geometries.Count();
                }
                centroid=new Geometry(wkbGeometryType.wkbPoint);
                centroid.SetPoint(0, destCoord);
            }
            return centroid;
        }
    }
}
