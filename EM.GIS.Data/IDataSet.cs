using EM.GIS.Geometries;
using EM.GIS.Projection;
using System;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据集
    /// </summary>
    public interface IDataSet:IDisposable
    {
        /// <summary>
        /// 范围
        /// </summary>
        IExtent Extent { get; }
        /// <summary>
        /// 路径
        /// </summary>
        string Filename { get; set; }
        /// <summary>
        /// 相对路径
        /// </summary>
        string RelativeFilename { get; }
        /// <summary>
        /// 已经释放
        /// </summary>
        bool IsDisposed { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 投影
        /// </summary>
        ProjectionInfo Projection { get; }
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 保存
        /// </summary>
        void Save();
        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="overwrite"></param>
        void SaveAs(string filename,bool overwrite);
    }
}