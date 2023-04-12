using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 矢量图层元数据
    /// </summary>
    public class VectorMetadata
    {
        /// <summary>
        /// 矢量图层
        /// </summary>
        public List<VectorLayer> vector_layers { get; } = new List<VectorLayer>();
        /// <summary>
        /// 矢量图层属性
        /// </summary>
        public List<Tilestat> tilestats { get; } = new List<Tilestat>();
        
        public override string ToString()
        {
            var str= JsonSerializer.Serialize(this);
            return str;
        }
    }
}
