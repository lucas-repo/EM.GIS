using EM.GIS.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace EM.GIS.Controls
{
    /// <summary>
    /// app管理接口
    /// </summary>
    public interface IAppManager
    {
        /// <summary>
        /// 进度处理
        /// </summary>
        IProgressHandler ProgressHandler { get; set; }
        /// <summary>
        /// 地图
        /// </summary>
        IMap Map { get; set; }
        /// <summary>
        /// 图例
        /// </summary>
        ILegend Legend { get; set; }
        /// <summary>
        /// 插件
        /// </summary>
        [Browsable(false)]
        [ImportMany(AllowRecomposition = true)]
        IEnumerable<IPlugin> Plugins { get; }
        /// <summary>
        /// 根目录
        /// </summary>
        string BaseDirectory { get; set; }
        /// <summary>
        /// 扩展目录(相对路径)
        /// </summary>
        List<string> Directories { get; }
        /// <summary>
        /// 读取插件
        /// </summary>
        void LoadPlugins();
    }
}
