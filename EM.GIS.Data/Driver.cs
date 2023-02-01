namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动
    /// </summary>
    public abstract class Driver:IDriver
    {
        /// <inheritdoc/>
        public ProgressDelegate Progress { get; set; }
        /// <inheritdoc/>
        public string Name { get; set; }=string.Empty;
        /// <inheritdoc/>
        public string Discription { get; set; } = string.Empty;

        /// <inheritdoc/>
        public abstract IDataSet? GetDataSet(string path);
    }
}