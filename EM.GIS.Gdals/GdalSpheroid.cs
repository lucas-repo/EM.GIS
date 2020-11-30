using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    internal class GdalSpheroid : Spheroid
    {
        public SpatialReference SpatialReference { get; set; }
        public override string Name
        {
            get => SpatialReference?.GetAttrValue("SPHEROID", 0);
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetAttrValue("SPHEROID", value);
                }
            }
        }
        public override double EquatorialRadius
        {
            get
            {
                double value = 0;
                if (SpatialReference != null)
                {
                    value = SpatialReference.GetSemiMajor();
                }
                return value;
            }
        }
        public override double PolarRadius
        {
            get
            {
                double value = 0;
                if (SpatialReference != null)
                {
                    value = SpatialReference.GetSemiMinor(); 
                }
                return value;
            }
        }
        public override double InverseFlattening
        {
            get
            {
                double value = 0;
                if (SpatialReference != null)
                {
                    value = SpatialReference.GetInvFlattening();
                }
                return value;
            }
        }
        public GdalSpheroid(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
    }
}