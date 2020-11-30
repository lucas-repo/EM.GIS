using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 驱动基类
    /// </summary>
    public abstract class Driver : IDriver
    {
        public IProgressHandler ProgressHandler { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }

        public virtual bool CopyFiles(string srcFileName, string destFileName)
        {
            throw new NotImplementedException();
        }

        public virtual bool Delete(string fileName)
        {
            throw new NotImplementedException();
        }

        public virtual List<string> GetReadableFileExtensions()
        {
            throw new NotImplementedException();
        }
        public virtual List<string> GetWritableFileExtensions()
        {
            throw new NotImplementedException();
        }

        public virtual IDataSet Open(string fileName, bool update)
        {
            throw new NotImplementedException();
        }

        public virtual bool Rename(string srcFileName, string destFileName)
        {
            throw new NotImplementedException();
        }
    }
}
