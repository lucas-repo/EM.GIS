using System;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 可显示进度接口
    /// </summary>
    public interface IReportable
    {
        /// <summary>
        /// 进度委托
        /// </summary>
        Action<string, int> ProgressAction { get; set; }
    }
}