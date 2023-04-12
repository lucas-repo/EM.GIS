using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 瓦片索引
    /// </summary>
    public abstract class Indexable:Record
    {
        /// <summary>
        /// 级别
        /// </summary>
        [Field("zoom_level", SQLites.FieldType.INTEGER,indexName: "tile_index")]
        public int Level { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        [Field("tile_column", SQLites.FieldType.INTEGER, indexName: "tile_index")]
        public int Column { get; set; }
        /// <summary>
        /// 行
        /// </summary>
        [Field("tile_row", SQLites.FieldType.INTEGER, indexName: "tile_index")]
        public int Row { get; set; }
    }
}
