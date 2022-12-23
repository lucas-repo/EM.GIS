using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 进度委托
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="percent">进度百分比</param>
    public delegate void ProgressDelegate(string message,int percent);
}
