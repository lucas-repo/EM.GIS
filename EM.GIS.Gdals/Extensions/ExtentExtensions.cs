using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class ExtentExtensions
    {
        #region 范围

        public static OSGeo.OGR.Geometry ToGeometry(this Envelope envelope)
        {
            var ring = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLinearRing);
            ring.AddPoint_2D(envelope.MinX, envelope.MinY);
            ring.AddPoint_2D(envelope.MaxX, envelope.MinY);
            ring.AddPoint_2D(envelope.MaxX, envelope.MaxY);
            ring.AddPoint_2D(envelope.MinX, envelope.MaxY);
            ring.CloseRings();
            var polygon = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
            polygon.AddGeometry(ring);
            return polygon;
        }
        public static IExtent ToExtent(this Envelope envelope)
        {
            IExtent extent = new Extent(envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY);
            return extent;
        }
        /// <summary>
        /// 转Envelope
        /// </summary>
        /// <returns></returns>
        public static Envelope ToEnvelope(this IExtent extent)
        {
            Envelope envelope = new Envelope();
            if (!double.IsNaN(extent.MinX))
            {
                envelope.MinX = extent.MinX;
                envelope.MinY = extent.MinY;
                envelope.MaxX = extent.MaxX;
                envelope.MaxY = extent.MaxY;
            }
            return envelope;
        }
        /// <summary>
        /// 是否包含范围
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool Contains(this IExtent extent, Envelope env)
        {
            if (Equals(env, null)) throw new ArgumentNullException(nameof(env));

            return extent.Contains(env.MinX, env.MaxX, env.MinY, env.MaxY);
        }
        /// <summary>
        /// 是否相交
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool Intersects(this IExtent extent, Envelope env)
        {
            if (Equals(env, null)) throw new ArgumentNullException(nameof(env));

            return extent.Intersects(env.MinX, env.MaxX, env.MinY, env.MaxY);
        }
        /// <summary>
        /// 是否包含于
        /// </summary>
        /// <param name="extent"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static bool Within(this IExtent extent, Envelope env)
        {
            if (Equals(env, null)) throw new ArgumentNullException(nameof(env));

            return extent.Within(env.MinX, env.MaxX, env.MinY, env.MaxY);
        }
        public static OSGeo.OGR.Geometry ToGeometry(this IExtent extent)
        {
            var ring = new OSGeo.OGR.Geometry(wkbGeometryType.wkbLinearRing);
            ring.AddPoint_2D(extent.MinX, extent.MinY);
            ring.AddPoint_2D(extent.MaxX, extent.MinY);
            ring.AddPoint_2D(extent.MaxX, extent.MaxY);
            ring.AddPoint_2D(extent.MinX, extent.MaxY);
            ring.CloseRings();
            var polygon = new OSGeo.OGR.Geometry(wkbGeometryType.wkbPolygon);
            polygon.AddGeometry(ring);
            return polygon;
        }
        #endregion
    }
}
