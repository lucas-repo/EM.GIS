using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    internal class GdalUnit : AngularUnit
    {
        public SpatialReference SpatialReference { get; set; }

        public override string Name
        {
            get => SpatialReference?.GetAttrValue("UNIT", 0);
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetAttrValue("UNIT", value);
                }
            }
        }
        public override double Radians
        {
            get
            {
                double value = 0;
                if (SpatialReference != null)
                {
                    value = SpatialReference.GetProjParm("UNIT", double.NaN);
                }
                return value;
            }
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetProjParm("UNIT", value);
                }
            }
        }
        public GdalUnit(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
    }
}