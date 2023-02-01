namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// 进度
        /// </summary>
        ProgressDelegate Progress { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        string Discription { get; set; }
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>数据集</returns>
        IDataSet? GetDataSet(string path);
    }
}