namespace EM.GIS.Symbology
{
    public interface IPolygonScheme:IFeatureScheme
    {
        new IPolygonCategoryCollection Categories { get; set; }
    }
}