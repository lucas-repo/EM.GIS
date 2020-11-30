using EM.GIS.Projection;
using OSGeo.OSR;

namespace EM.GIS.Gdals
{
    public class GdalGeographicInfo : GeographicInfo
    {
        public SpatialReference SpatialReference { get; set; }
        public override Datum Datum 
        {
            get
            {
                if (base.Datum == null)
                {
                    base.Datum = new GdalDatum(SpatialReference);
                }
                return base.Datum;
            }
            set => base.Datum = value;
        }
        public override Meridian Meridian
        {
            get
            {
                if (base.Meridian == null)
                {
                    base.Meridian = new GdalMeridian(SpatialReference);
                }
                return base.Meridian;
            }
            set => base.Meridian = value;
        }
        public override AngularUnit Unit
        {
            get
            {
                if (base.Unit == null)
                {
                    base.Unit = new GdalUnit(SpatialReference);
                }
                return base.Unit;
            }
            set => base.Unit = value;
        }
        public override string Name
        {
            get => SpatialReference?.GetAttrValue("GEOGCS", 0);
            set
            {
                if (SpatialReference != null)
                {
                    SpatialReference.SetAttrValue("GEOGCS", value);
                }
            }
        }
        
        public GdalGeographicInfo(SpatialReference spatialReference)
        {
            SpatialReference = spatialReference;
        }
    }
}