using System.ComponentModel.Composition;
namespace EM.GIS.Controls
{
    /// <summary>
    /// 插件接口
    /// </summary>
    [InheritedExport]
    public interface IPlugin
    {
        #region Properties
        /// <summary>
        /// app管理器
        /// </summary>
        [Import]
        IAppManager App { get; set; }

        /// <summary>
        /// 限定名称
        /// </summary>
        string AssemblyQualifiedName { get; }

        /// <summary>
        /// 作者
        /// </summary>
        string Author { get; }

        /// <summary>
        /// 创建日期
        /// </summary>
        string BuildDate { get; }

        /// <summary>
        /// 是否允许冻结
        /// </summary>
        bool DeactivationAllowed { get; }

        /// <summary>
        /// 描述
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 是否已激活
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 优先级别
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 激活
        /// </summary>
        void Activate();

        /// <summary>
        /// 冻结
        /// </summary>
        void Deactivate();

        #endregion
    }
}
