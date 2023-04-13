using BruTile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 两个个相邻的瓦片信息
    /// </summary>
    public class AdjacentTile
    {
        /// <summary>
        /// 左边的瓦片
        /// </summary>
        public TileInfo? Left { get; set; }
        /// <summary>
        /// 右边的瓦片
        /// </summary>
        public TileInfo? Right { get; set; }
        /// <summary>
        /// 瓦片集合
        /// </summary>
        public IEnumerable<TileInfo> TileInfos
        {
            get
            {
                if (Left != null)
                {
                    yield return Left;
                }
                if (Right != null)
                {
                    yield return Right;
                }
            }
        }

        /// <summary>
        /// 初始化<see cref="AdjacentTile"/>
        /// </summary>
        /// <param name="left">左边的瓦片</param>
        /// <param name="right">右边的瓦片</param>
        public AdjacentTile(TileInfo? left, TileInfo? right)
        {
            Left = left;
            Right = right;
        }
        /// <summary>
        /// 根据瓦片信息集合，计算相邻瓦片集合
        /// </summary>
        /// <param name="tileInfos">瓦片信息集合</param>
        /// <returns>相邻瓦片集合</returns>
        public static List<AdjacentTile> GetAdjacentTiles(IEnumerable<TileInfo> tileInfos)
        {
            var ret = new List<AdjacentTile>();
            if (tileInfos.Count() > 0)
            {
                var groupedTileInfos = tileInfos.GroupBy(x => x.Index.Level);
                foreach (var groupedTileInfo in groupedTileInfos)
                {
                    var orderedTileInfos = groupedTileInfo.OrderBy(x => x.Index);
                    foreach (var tileInfo in orderedTileInfos)
                    {
                        var tileIndex = tileInfo.Index;
                        bool quadTileIsExist = false;
                        foreach (var quadTile in ret)
                        {
                            if (quadTile.Left?.Index.Level == tileIndex.Level && quadTile.Left.Index.Row == tileIndex.Row && quadTile.Left.Index.Col == tileIndex.Col - 1)//为右上角
                            {
                                quadTile.Right = tileInfo;
                                quadTileIsExist = true;
                            }
                        }
                        if (!quadTileIsExist)
                        {
                            var tile = new AdjacentTile(tileInfo, null);
                            ret.Add(tile);
                        }
                    }
                }
            }
            return ret;
        }
    }
}
