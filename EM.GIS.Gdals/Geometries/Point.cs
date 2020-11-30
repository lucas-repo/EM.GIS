using EM.GIS.Geometries;
using System;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class Point : Geometry, IPoint
    {
        public double X { get => OgrGeometry.GetX(0); set => OgrGeometry.SetPoint_2D(0, value, Y); }
        public double Y { get => OgrGeometry.GetY(0); set => OgrGeometry.SetPoint_2D(0, X, value); }
        public double Z { get => OgrGeometry.GetZ(0); set => OgrGeometry.SetPointZM(0, X, Y, value, M); }
        public double M { get => OgrGeometry.GetM(0); set => OgrGeometry.SetPointM(0, X, Y, value); }
        public Point(OSGeo.OGR.Geometry geometry) : base(geometry)
        {
            if (geometry.GetGeometryType() != OSGeo.OGR.wkbGeometryType.wkbPoint)
            {
                throw new Exception("非法的参数");
            }
        }
        public Point() : this(double.NaN, double.NaN)
        { }
        public Point(double x, double y, double z = double.NaN, double m = double.NaN) : base(GeometryExtensions.ToPointGeometry(x,y,z,m))
        { }
        public Point(ICoordinate coord) : base(coord.ToPointGeometry())
        { }

        public override object Clone()
        {
            return new Point(OgrGeometry.Clone());
        }
    }
}
