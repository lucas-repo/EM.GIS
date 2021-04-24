using EM.GIS.Symbology;
using System.Collections.ObjectModel;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 命令工厂
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// 命令
        /// </summary>
        ObservableCollection<IBaseCommand> Commands { get; }
    }
}