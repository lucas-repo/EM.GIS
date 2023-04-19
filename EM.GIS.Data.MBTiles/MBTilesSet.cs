using BruTile;
using EM.GIS.Geometries;
using EM.GIS.MBTiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Data.MBTiles
{
    /// <summary>
    /// MBTiles数据集
    /// </summary>
    public class MBTilesSet : TileSet
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public MBTilesContext Context { get; }

        public MBTilesSet(MBTilesContext context) : base(new MBTilesSource(context))
        {
            Context = context;
        }

        /// <summary>
        /// 写入图片
        /// </summary>
        /// <param name="tileIndex">索引</param>
        /// <param name="buffer">图片缓存</param>
        public void Write(TileIndex tileIndex, byte[] buffer)
        {
            Tile tile = new Tile()
            {
                Level = tileIndex.Level,
                Column = tileIndex.Col,
                Row = tileIndex.Row,
                Datas = buffer
            };
            Context.Tiles.Update(tile);
        }
    }
}
