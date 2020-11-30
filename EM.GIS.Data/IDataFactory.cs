using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 数据工厂接口
    /// </summary>
    public interface IDataFactory
    {
        /// <summary>
        /// 数据驱动工厂
        /// </summary>
        IDriverFactory DriverFactory { get; }
        /// <summary>
        /// 几何工厂
        /// </summary>
        IGeometryFactory GeometryFactory { get; set; }
        /// <summary>
        /// 进度
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
    }
}
