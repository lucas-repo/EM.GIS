namespace EM.GIS.Symbology
{
    public interface IPointScheme : IFeatureScheme
    {
        new IPointCategoryCollection Categories { get; set; }
    }
}