using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 进度处理类
    /// </summary>
    public class ProgressHandler : IProgressHandler
    {
        public ProgressDelegate Progress { get; set; }
    }
}
