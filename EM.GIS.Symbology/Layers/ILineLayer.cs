namespace EM.GIS.Symbology
{
    public interface ILineLayer:IFeatureLayer
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new ILineCategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new ILineCategory DefaultCategory { get; set; }
    }
}