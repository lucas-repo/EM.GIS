﻿using EM.SQLites;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 瓦片表
    /// </summary>
    public class Tiles : SQLiteTable<Tile>
    {
        public Tiles(DbConnection connection) : base(connection, "tiles")
        {
        }
        
    }
}