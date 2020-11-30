using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    internal class GdalMeridian : Meridian
    {
        public SpatialReference SpatialReference { get; set; }

        public override string Name
        {
            get => SpatialReference?.GetAttrValue("PRIMEM", 0);
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetAttrValue("PRIMEM", value);
                }
            }
        }
        public override double Longitude
        {
            get
            {
                double value = 0;
                if (SpatialReference != null)
                {
                    value = SpatialReference.GetProjParm("PRIMEM",double.NaN);
                }
                return value;
            }
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetProjParm("PRIMEM", value);
                }
            }
        }
        public GdalMeridian(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
    }
}