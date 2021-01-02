namespace EM.GIS.Data
{
    /// <summary>
    /// An abreviated list for quick classification
    /// </summary>
    public enum FeatureType
    {
        /// <summary>
        /// 未指明或自定义
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 点
        /// </summary>
        Point = 1,

        /// <summary>
        /// 线
        /// </summary>
        Polyline = 2,

        /// <summary>
        /// 面
        /// </summary>
        Polygon = 3,

        /// <summary>
        /// 多点
        /// </summary>
        MultiPoint = 4
    }
}