using System.Collections.Generic;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动
    /// </summary>
    public abstract class Driver:IDriver
    {
        /// <inheritdoc/>
        public string Name { get; set; }=string.Empty;
        /// <inheritdoc/>
        public string Discription { get; set; } = string.Empty;
        /// <inheritdoc/>
        public string Extensions { get; protected set; } =string.Empty;
        /// <inheritdoc/>
        public string Filter { get; set; } = string.Empty;

        /// <inheritdoc/>
        public abstract IDataSet? Open(string path);
    }
}