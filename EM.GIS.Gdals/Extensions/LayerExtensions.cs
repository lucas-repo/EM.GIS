using EM.GIS.Geometries;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class LayerExtensions
    {
        public static Envelope GetEnvelope(this Layer layer)
        {
            Envelope envelope = null;
            if (layer != null)
            {
                envelope = new Envelope();
                var ret= layer.GetExtent(envelope, 1);
            }
            return envelope;
        }
        public static IExtent GetExtent(this Layer layer)
        {
            IExtent extent = layer?.GetEnvelope().ToExtent(); 
            return extent;
        }
    }
}
