using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可改变接口
    /// </summary>
    public interface IChangeable
    {
        /// <summary>
        /// 是否暂停改变
        /// </summary>
        bool ChangesSuspended { get;}
        /// <summary>
        /// 改变事件
        /// </summary>
        event EventHandler Changed;
        /// <summary>
        /// 恢复改变
        /// </summary>
        void ResumeChanges();
        /// <summary>
        /// 暂停改变
        /// </summary>
        void SuspendChanges();
    }
}