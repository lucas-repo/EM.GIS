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
        public static ICoordinate TransformPoint(this CoordinateTransformation coordinateTransformation, ICoordinate coord)
        {
            ICoordinate destCoord = null;
            if (coordinateTransformation != null && coord != null)
            {
                var array = coord.ToDoubleArray(coord.NumOrdinates);
                coordinateTransformation.TransformPoint(array);
                destCoord = array.ToCoordinate();
            }
            return destCoord;
        }
        public static List<ICoordinate> TransformPoints(this CoordinateTransformation coordinateTransformation, IEnumerable<Coordinate> coords)
        {
            List<ICoordinate> destCoords = new List<ICoordinate>();
            if (coordinateTransformation != null && coords != null  )
            {
                var firstCoord = coords.FirstOrDefault();
                if (firstCoord!=null)
                {
                    var count = coords.Count();
                    var dimension = firstCoord.NumOrdinates;
                    if (dimension >= 2)
                    {
                        double[] xs = new double[count];
                        double[] ys = new double[count];
                        double[] zs = new double[count];
                        int i = 0;
                        foreach (var coord in coords)
                        {
                            xs[i] = coord[0];
                            ys[i] = coord[1];
                            if (dimension > 2)
                            {
                                zs[i] = coord[2];
                            }
                            i++;
                        }
                        coordinateTransformation.TransformPoints(count,xs,ys,zs);
                        for (int j = 0; j < xs.Length; j++)
                        {
                            var coord = new Coordinate(xs[j], ys[j]);
                            if (dimension > 2)
                            {
                                coord.Z = zs[j];
                            }
                            destCoords.Add(coord);
                        }
                    }
                }
            }
            return destCoords;
        }
        public static Extent TransformExtent(this CoordinateTransformation coordinateTransformation, Extent extent)
        {
            Extent destExtent = null;
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
                destExtent = new Extent(minX, minY, maxX, maxY);
            }
            return destExtent;
        }
    }
}
