using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    internal class GdalDatum : Datum
    {
        public SpatialReference SpatialReference { get; set; }
        public override string Name
        {
            get => SpatialReference?.GetAttrValue("DATUM",0);
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetAttrValue("DATUM", value );
                }
            }
        }
        public override DatumType DatumType { get => base.DatumType; set => base.DatumType = value; }
        public override double[] ToWGS84 
        {
            get
            {
                double[] toWGS84 = new double[7];
                if (SpatialReference != null)
                {
                    var ret = SpatialReference.GetTOWGS84(toWGS84); 
                }
                return toWGS84;
            }
            set
            {
                if (SpatialReference != null)
                {
                    var ret = SpatialReference.SetTOWGS84(value[0], value[1], value[2], value[3], value[4], value[5], value[6]);
                }
            }
        }
        public override Spheroid Spheroid
        {
            get
            {
                if (base.Spheroid == null)
                {
                    base.Spheroid = new GdalSpheroid(SpatialReference);
                }
                return base.Spheroid;
            }
            set => base.Spheroid = value;
        }
        public GdalDatum(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference; 
        }
    }
}