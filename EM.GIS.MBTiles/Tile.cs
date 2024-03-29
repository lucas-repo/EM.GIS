﻿using EM.SQLites;
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
    public class Tile : Indexable
    {
        /// <summary>
        /// 二进制数据
        /// </summary>
        [Field("tile_data", SQLites.FieldType.BLOB)]
        public byte[] Datas { get; set; } = new byte[0];
    }
}
