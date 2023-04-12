using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 栅格数据
    /// </summary>
    public class GridData : Indexable
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Field("key_name")]
        public string KeyName { get; set; } = string.Empty;
        /// <summary>
        /// 值
        /// </summary>
        [Field("key_json")]
        public string KeyJson { get; set; } = string.Empty;
    }
}
