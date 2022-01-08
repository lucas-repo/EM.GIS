using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class GdalLayerExtensions
    {
        public static Envelope GetEnvelope(this Layer layer)
        {
            Envelope envelope = null;
            if (layer != null)
            {
                envelope = new Envelope();
                var ret = layer.GetExtent(envelope, 1);
            }
            return envelope;
        }
    }
}
