using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 范围扩展
    /// </summary>
    public static class EnvelopeExtensions
    {
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
    }
}
