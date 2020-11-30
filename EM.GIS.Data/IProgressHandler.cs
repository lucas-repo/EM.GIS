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
        /// 汇报进度方法
        /// </summary>
        /// <param name="percent">百分比</param>
        /// <param name="message">消息</param>
        void Progress(int percent,string message=null);
    }
}
