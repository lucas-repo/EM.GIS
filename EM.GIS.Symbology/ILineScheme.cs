namespace EM.GIS.Symbology
{
    public interface ILineScheme:IFeatureScheme
    {
         new ILineCategoryCollection Categories { get; set; }
    }
}