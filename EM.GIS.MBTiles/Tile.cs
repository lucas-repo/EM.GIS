using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 瓦片
    /// </summary>
    public class Tile:Record
    {
        /// <summary>
        /// 级别
        /// </summary>
        [Field("zoom_level",FieldType.INTEGER)]
        public int Level { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        [Field("tile_column", FieldType.INTEGER)]
        public int Column { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        [Field("tile_row", FieldType.INTEGER)]
        public int Row { get; set; }
        /// <summary>
        /// 二进制数据
        /// </summary>
        [Field("tile_data",FieldType.BLOB)]
        public byte[] Datas { get; set; }
    }
}
