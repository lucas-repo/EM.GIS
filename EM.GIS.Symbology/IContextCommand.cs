using System.Windows.Input;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 命令
    /// </summary>
    public interface IContextCommand : ICommand
    {
        /// <summary>
        /// 标题
        /// </summary>
        string Header { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 提示
        /// </summary>
        string ToolTip { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        object? Image { get; set; }
        /// <summary>
        /// 大图标
        /// </summary>
        object? LargeImage { get; set; }
    }
}