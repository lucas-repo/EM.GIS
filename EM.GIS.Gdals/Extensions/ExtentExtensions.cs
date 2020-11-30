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

        public static double Width(this Envelope envelope)
        {
            return envelope != null ? envelope.MaxX - envelope.MinX : 0;
        }
        public static double[] Center(this Envelope envelope)
        {
            double[] center = null;
            if (envelope != null)
            {
                center = new double[2];
                center[0] = (envelope.MinX + envelope.MaxX) / 2;
                center[1] = (envelope.MinY + envelope.MaxY) / 2;
            }
            return center;
        }
        public static void SetCenter(this Envelope envelope, double[] center, double width, double height)
        {
            if (Equals(center, null)) throw new ArgumentNullException(nameof(center));

            envelope.SetCenter(center[0], center[1], width, height);
        }
        public static void SetCenter(this Envelope envelope, double centerX, double centerY, double width, double height)
        {
            envelope.MinX = centerX - (width / 2);
            envelope.MaxX = centerX + (width / 2);
            envelope.MinY = centerY - (height / 2);
            envelope.MaxY = centerY + (height / 2);
        }
        public static Envelope Copy(this Envelope envelope)
        {
            Envelope copy = null;
            if (envelope != null)
            {
                copy = new Envelope()
                {
                    MinX = envelope.MinX,
                    MaxX = envelope.MaxX,
                    MinY = envelope.MinY,
                    MaxY = envelope.MaxY
                };
            }
            return copy;
        }
        public static double Height(this Envelope envelope)
        {
            return envelope != null ? envelope.MaxY - envelope.MinY : 0;
        }
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
        public static void ExpandToInclude(this Envelope srcEnvelope, Envelope destEnvelope)
        {
            if (srcEnvelope == null || destEnvelope == null)
            {
                return;
            }
            srcEnvelope.MinX = Math.Min(srcEnvelope.MinX, destEnvelope.MinX);
            srcEnvelope.MinY = Math.Min(srcEnvelope.MinY, destEnvelope.MinY);
            srcEnvelope.MaxX = Math.Max(srcEnvelope.MaxX, destEnvelope.MaxX);
            srcEnvelope.MaxY = Math.Min(srcEnvelope.MaxY, destEnvelope.MaxY);
        }
        public static void ExpandBy(this Envelope envelope, double deltaX, double deltaY)
        {
            if (envelope != null)
            {
                envelope.MinX -= deltaX;
                envelope.MaxX += deltaX;
                envelope.MinY -= deltaY;
                envelope.MaxY += deltaY;
                if (envelope.MinX > envelope.MaxX || envelope.MinY > envelope.MaxY)
                {
                    envelope.MinX = double.NaN;
                    envelope.MaxX = double.NaN;
                    envelope.MinY = double.NaN;
                    envelope.MaxY = double.NaN;
                }
            }
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
