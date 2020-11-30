using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class LineString : Geometry, ILineString
    {
        public IPoint StartPoint
        {
            get
            {
                IPoint point = null;
                if (OgrGeometry != null)
                {
                    if (OgrGeometry.GetPointCount() > 0)
                    {
                        point = OgrGeometry.GetPoint(0);
                    }
                }
                return point;
            }
        }

        public IPoint EndPoint
        {
            get
            {
                IPoint point = null;
                if (OgrGeometry != null)
                {
                    var count = OgrGeometry.GetPointCount();
                    if (count > 0)
                    {
                        point = OgrGeometry.GetPoint(count - 1);
                    }
                }
                return point;
            }
        }

        public double Angle
        {
            get
            {
                var startPoint = StartPoint;
                var endPoint = EndPoint;
                var x = endPoint.X - startPoint.X;
                var y = endPoint.Y - startPoint.Y;

                if (x == 0)
                {
                    return y > 0 ? 90 : 270;
                }
                var a = Math.Atan(y / x);
                var ret = a * 180 / Math.PI;
                if (x < 0)
                {
                    ret = 180 + ret;
                }
                if (ret > 360)
                {
                    ret -= 360;
                }
                if (ret < 0)
                {
                    ret += 360;
                }
                return ret;
            }
        }

        public LineString(OSGeo.OGR.Geometry geometry) : base(geometry)
        {
        }
        public LineString(IEnumerable<ICoordinate> coordinates) : base(coordinates.ToLineStringGeometry())
        {
        }

        public override object Clone()
        {
            return new LineString(OgrGeometry.Clone());
        }
    }
}
