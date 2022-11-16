using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可取消的接口
    /// </summary>
    public interface ICancelable
    {
        /// <summary>
        /// 正在工作
        /// </summary>
        bool IsWorking { get; }
        /// <summary>
        /// 取消标记源
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; set; }
    }
}
