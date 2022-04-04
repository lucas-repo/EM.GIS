using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 进度接口
    /// </summary>
    public interface IProgressHandler
    {
        /// <summary>
        /// 进度委托
        /// </summary>
        ProgressDelegate Progress { get; set; }
    }
}
