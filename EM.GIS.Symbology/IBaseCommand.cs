using System.Windows.Input;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface IBaseCommand : ICommand
    {
        /// <summary>
        /// 标题
        /// </summary>
        object Header { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        object ToolTip { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        object Icon { get; set; }
        /// <summary>
        /// 大图标
        /// </summary>
        object LargeIcon { get; set; }
    }
}