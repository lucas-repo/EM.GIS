using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 环境管理器
    /// </summary>
    public interface IRuntimeManager
    {
        /// <summary>
        /// 是否已注册
        /// </summary>
        bool IsRegisted { get; }
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        bool Register();
        /// <summary>
        /// 取消注册
        /// </summary>
        void Unregister();
    }
}
