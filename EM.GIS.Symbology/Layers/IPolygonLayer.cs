namespace EM.GIS.Symbology
{
    /// <summary>
    /// 面图层
    /// </summary>
    public interface IPolygonLayer:IFeatureLayer
    {
        /// <summary>
        /// 分类集合
        /// </summary>
        new IPolygonCategoryCollection Categories { get; }
        /// <summary>
        /// 默认分类
        /// </summary>
        new IPolygonCategory DefaultCategory { get; set; }
    }
}