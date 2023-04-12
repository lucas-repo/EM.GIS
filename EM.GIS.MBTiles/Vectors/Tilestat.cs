using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 矢量图层属性
    /// </summary>
    public class Tilestat
    {
        /// <summary>
        /// 个数
        /// </summary>
        public int layerCount { get; set; }
        public List<Layer> layers { get; } = new List<Layer>();
    }
}
