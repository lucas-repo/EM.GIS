using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 栅格
    /// </summary>
    public class Grid : Indexable
    {
        /// <summary>
        /// 二进制数据
        /// </summary>
        [Field("grid ", SQLites.FieldType.BLOB)]
        public byte[] Datas { get; set; } = new byte[0];
    }
}
