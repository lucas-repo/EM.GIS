using System;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 可显示进度接口
    /// </summary>
    public interface IReportable
    {
        /// <summary>
        /// 是否空闲的
        /// </summary>
        bool IsFree { get; set; }
        /// <summary>
        /// 进度委托
        /// </summary>
        Action<string, int> ProgressAction { get; set; }
        /// <summary>
        /// 取消
        /// </summary>
        void Cancel();
    }
}