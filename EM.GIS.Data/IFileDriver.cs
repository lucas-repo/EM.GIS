using System.Collections.Generic;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动接口
    /// </summary>
    public interface IFileDriver:IDriver
    {
        /// <summary>
        /// 打开
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="update">是否更新</param>
        /// <returns>数据集</returns>
        IDataSet Open(string fileName, bool update);
        /// <summary>
        /// 删除数据集
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>成功true反之false</returns>
        bool Delete(string fileName);
        /// <summary>
        /// 重命名数据集
        /// </summary>
        /// <param name="srcFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>成功true反之false</returns>
        bool Rename(string srcFileName,string destFileName);
        /// <summary>
        /// 复制数据集
        /// </summary>
        /// <param name="srcFileName">源文件名</param>
        /// <param name="destFileName">目标文件名</param>
        /// <returns>成功true反之false</returns>
        bool CopyFiles(string srcFileName, string destFileName);
        /// <summary>
        /// 获取可读文件扩展
        /// </summary>
        /// <returns>可读文件扩展</returns>
        List<string> GetReadableFileExtensions();
        /// <summary>
        /// 获取可写文件扩展
        /// </summary>
        /// <returns>可写文件扩展</returns>
        List<string> GetWritableFileExtensions();
    }
}
