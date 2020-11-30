namespace EM.GIS.Symbology
{
    public interface IPointLayer:IFeatureLayer
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new IPointCategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new IPointCategory DefaultCategory { get; set; }
    }
}