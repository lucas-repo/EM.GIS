using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 名称和值
    /// </summary>
    public class NameValue: Record
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Field("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 值
        /// </summary>
        [Field("name")]
        public string Value { get; set; } = string.Empty;
    }
}
