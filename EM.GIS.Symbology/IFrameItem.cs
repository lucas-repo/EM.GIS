namespace EM.GIS.Symbology
{
    /// <summary>
    /// 带框架的元素接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFrameItem<T> where T: IFrame
    {
        /// <summary>
        /// 框架
        /// </summary>
        T Frame { get; set; }
    }
}