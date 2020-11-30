namespace EM.GIS.Symbology
{
    /// <summary>
    /// 带父元素的接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParentItem<T>
    {
        /// <summary>
        /// 父元素
        /// </summary>
        T Parent { get; set; }
    }
}