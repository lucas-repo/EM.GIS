using EM.Bases;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Gdals
{
    [Serializable]
    public class Geometry : BaseCopy, IGeometry
    {
        private OSGeo.OGR.Geometry _ogrGeometry;
        private bool disposedValue;

        public OSGeo.OGR.Geometry OgrGeometry
        {
            get { return _ogrGeometry; }
            protected set
            {
                SetProperty(ref _ogrGeometry, value);
            }
        }

        public IExtent GetExtent()
        {
            IExtent extent = null;
            if (OgrGeometry != null)
            {
                OSGeo.OGR.Envelope envelope = new OSGeo.OGR.Envelope();
                OgrGeometry.GetEnvelope(envelope);
                extent = envelope.ToExtent();
            }
            return extent;
        }

        public GeometryType GeometryType
        {
            get
            {
                GeometryType geometryType = GeometryType.Unknown;
                if (OgrGeometry != null)
                {
                    geometryType = OgrGeometry.GetGeometryType().ToGeometryType();
                }
                return geometryType;
            }
        }

        /// <inheritdoc/>
        public int GeometryCount => OgrGeometry != null ? OgrGeometry.GetGeometryCount() : 0;

        /// <inheritdoc/>
        public int CoordinateCount => OgrGeometry != null ? OgrGeometry.GetPointCount() : 0;

        public Geometry(OSGeo.OGR.Geometry geometry)
        {
            if (geometry == null)
            {
                throw new Exception("geometry不能为空");
            }
            OgrGeometry = geometry;
        }

        private void Coordinates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ICoordinate item in e.NewItems)
                    {
                        switch (item.Dimension)
                        {
                            case 2:
                                OgrGeometry.AddPoint_2D(item.X, item.Y);
                                break;
                            case 3:
                                OgrGeometry.AddPoint(item.X, item.Y, item.Z);
                                break;
                            case 4:
                                OgrGeometry.AddPointZM(item.X, item.Y, item.Z, item.M);
                                break;
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Geometries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Geometry item in e.NewItems)
                    {
                        OgrGeometry.AddGeometry(item.OgrGeometry);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public double Area() => OgrGeometry == null ? 0 : OgrGeometry.Area();

        public double Length() => OgrGeometry == null ? 0 : OgrGeometry.Length();

        public bool IsEmpty() => OgrGeometry == null ? true : OgrGeometry.IsEmpty();

        public string ToWkt()
        {
            var ret = OgrGeometry.ExportToWkt(out string wkt);
            return wkt;
        }
        public override string ToString()
        {
            return ToWkt();
        }

        public bool Contains(IGeometry g)
        {
            bool ret = false;
            if (g != null)
            {
                ret = OgrGeometry.Contains(g.ToGeometry());
            }
            return ret;
        }

        public bool Intersects(IGeometry g)
        {
            bool ret = false;
            if (g is Geometry geometry)
            {
                ret = OgrGeometry.Intersects(geometry.OgrGeometry);
            }
            return ret;
        }

        public IGeometry Intersection(IGeometry other)
        {
            IGeometry destGeometry = null;
            if (other != null)
            {
                destGeometry = OgrGeometry.Intersection(other.ToGeometry()).ToGeometry();
            }
            return destGeometry;
        }

        public double Distance(IGeometry g)
        {
            double ret = 0;
            if (g != null)
            {
                ret = OgrGeometry.Distance(g.ToGeometry());
            }
            return ret;
        }

        public IGeometry Union(IGeometry other)
        {
            IGeometry destGeometry = null;
            if (other != null)
            {
                var geometry = OgrGeometry.Union(other.ToGeometry());
                destGeometry = geometry.ToGeometry();
            }
            return destGeometry;
        }

        public double Distance(ICoordinate coord)
        {
            double ret = 0;
            if (coord != null)
            {
                ret = OgrGeometry.Distance(coord.ToOgrPoint());
            }
            return ret;
        }

        public override bool Equals(object obj)
        {
            bool ret = false;
            if (obj is Geometry geometry)
            {
                ret = OgrGeometry == geometry.OgrGeometry;
            }
            return ret;
        }

        public override int GetHashCode()
        {
            int hashCode = GeometryType.GetHashCode() ^ OgrGeometry.GetHashCode();
            return hashCode;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并替代终结器
                // TODO: 将大型字段设置为 null
                if (_ogrGeometry != null)
                {
                    _ogrGeometry.Dispose();
                    _ogrGeometry = null;
                }
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~Geometry()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        /// <inheritdoc/>
        public IGeometry? GetGeometry(int index)
        {
            IGeometry? ret = null;
            if (OgrGeometry != null && index >= 0 && OgrGeometry.GetGeometryCount() > index)
            {
                var osgGeo = OgrGeometry.GetGeometryRef(index);
                ret = new Geometry(osgGeo);
            }
            return ret;
        }

        /// <inheritdoc/>
        public ICoordinate? GetCoordinate(int index)
        {
            ICoordinate? ret = null;
            if (OgrGeometry != null && index >= 0 && OgrGeometry.GetPointCount() > index)
            {
                var dimension = OgrGeometry.GetCoordinateDimension();
                double[] argout = new double[dimension];
                OgrGeometry.GetPoint(index, argout);
                ret = new Coordinate(argout);
            }
            return ret;
        }
        protected override void OnCopy(object copy)
        {
            base.OnCopy(copy);
            if (copy is Geometry geometry)
            {
                geometry.OgrGeometry = OgrGeometry.Clone();
            }
        }
    }
}
