using EM.Bases;
using EM.GIS.Geometries;
using EM.GIS.Projections;
using System;
using System.ComponentModel;
using System.IO;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据集
    /// </summary>
    [Serializable]
    public abstract class DataSet : BaseCopy, IDataSet
    {

        #region Properties

        public virtual IExtent Extent { get;  set; }

        private string _filename;
        public string Filename
        {
            get => _filename;
            set
            {
                _filename = value;
                if (File.Exists(Filename))
                {
                    RelativeFilename = FilePathUtils.RelativePathTo(Filename);
                }
                else
                {
                    RelativeFilename = Filename;
                }
            }
        }

        public string Name { get; set; }

        public bool IsDisposed { get; private set; }

        #endregion

        public virtual string RelativeFilename { get; protected set; }

        public Projection Projection { get; set; }

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                IsDisposed = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DataSet()
        // {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        public virtual void Save() { }
        public virtual void SaveAs(string filename, bool overwrite) { }

        #endregion

    }
}

