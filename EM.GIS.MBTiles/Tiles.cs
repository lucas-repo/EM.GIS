using EM.SQLites;
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
        /// <summary>
        /// 获取瓦片
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="col">列</param>
        /// <param name="row">行</param>
        /// <returns>瓦片</returns>
        public Tile? GetObject(int level, int col, int row)
        {
            Tile? tile = null;
            if (Connection == null || string.IsNullOrEmpty(Name))
            {
                return tile;
            }
            var sql = $"SELECT * FROM {Name} WHERE zoom_level = {level} AND tile_column = {col} AND tile_row = {row}";
            var tiles= GetObjects(sql);
            if (tiles.Count == 1)
            {
                tile = tiles.First();
            }
            return tile;
        }
    }
}
