using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动接口
    /// </summary>
    [InheritedExport]
    public interface IDriver
    {
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        string Discription { get; set; }
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        IDataSet Open(string fileName, bool update);
        /// <summary>
        /// 删除数据集
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool Delete(string fileName);
        /// <summary>
        /// 重命名数据集
        /// </summary>
        /// <param name="srcFileName"></param>
        /// <param name="destFileName"></param>
        /// <returns></returns>
        bool Rename(string srcFileName,string destFileName);
        /// <summary>
        /// 复制数据集
        /// </summary>
        /// <param name="srcFileName"></param>
        /// <param name="destFileName"></param>
        /// <returns></returns>
        bool CopyFiles(string srcFileName, string destFileName);
        /// <summary>
        /// 获取可读文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetReadableFileExtensions();
        /// <summary>
        /// 获取可写文件扩展
        /// </summary>
        /// <returns></returns>
        List<string> GetWritableFileExtensions();
    }
}
