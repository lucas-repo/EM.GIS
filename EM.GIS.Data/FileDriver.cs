using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 驱动基类
    /// </summary>
    public abstract class FileDriver : Driver, IFileDriver
    {
        /// <inheritdoc/>
        public abstract bool CopyFiles(string srcFileName, string destFileName);

        /// <inheritdoc/>
        public abstract bool Delete(string fileName);

        /// <inheritdoc/>
        public abstract List<string> GetReadableFileExtensions();
        /// <inheritdoc/>
        public abstract List<string> GetWritableFileExtensions();

        /// <inheritdoc/>
        public abstract IDataSet? GetDataSet(string fileName, bool update);

        /// <inheritdoc/>
        public abstract bool Rename(string srcFileName, string destFileName);
        /// <inheritdoc/>
        public override IDataSet? GetDataSet(string path)
        {
            return GetDataSet(path, false);
        }
    }
}
