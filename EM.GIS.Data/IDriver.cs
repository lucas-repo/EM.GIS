using System.Collections.Generic;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据驱动
    /// </summary>
    public interface IDriver
    {
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        string Discription { get; set; }
        /// <summary>
        /// 扩展
        /// </summary>
        string Extensions { get; }
        /// <summary>
        /// 过滤
        /// </summary>
        string Filter { get; set; }
        /// <summary>
        /// 打开数据集
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>数据集</returns>
        IDataSet? Open(string path);
    }
}