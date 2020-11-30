using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 选择接口
    /// </summary>
    public interface ISelection:IChangeable
    {
        /// <summary>
        /// 范围
        /// </summary>
        IExtent IExtent { get; }
        /// <summary>
        /// 分类
        /// </summary>
        ICategory Category { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
    }
}