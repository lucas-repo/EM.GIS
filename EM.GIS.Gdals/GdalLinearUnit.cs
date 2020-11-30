using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    internal class GdalLinearUnit : LinearUnit
    {
        public SpatialReference SpatialReference { get; set; }

        public override string Name
        {
            get => SpatialReference?.GetLinearUnitsName();
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetLinearUnits(value, Meters);
                }
            }
        }
        public override double Meters
        {
            get
            {
                double meters = 0;
                if (SpatialReference != null)
                {
                    meters = SpatialReference.GetLinearUnits();
                }
                return meters;
            }
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetLinearUnits(Name, value);
                }
            }
        }
        public GdalLinearUnit(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
    }
}