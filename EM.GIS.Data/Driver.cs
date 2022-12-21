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
        public string Name { get; set; }
        /// <inheritdoc/>
        public string Discription { get; set; }

        /// <inheritdoc/>
        public abstract IDataSet Open(string path);
    }
}