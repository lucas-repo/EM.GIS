using BruTile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// 四个相邻的瓦片信息
    /// </summary>
    public class QuadTile
    {
        /// <summary>
        /// 左上角瓦片
        /// </summary>
        public TileInfo? TopLeft { get; set; }
        /// <summary>
        /// 右上角瓦片
        /// </summary>
        public TileInfo? TopRight { get; set; }
        /// <summary>
        /// 右下角瓦片
        /// </summary>
        public TileInfo? BottomRight { get; set; }
        /// <summary>
        /// 左下角瓦片
        /// </summary>
        public TileInfo? BottomLeft { get; set; }
        /// <summary>
        /// 瓦片集合
        /// </summary>
        public IEnumerable<TileInfo> TileInfos
        {
            get
            {
                if (TopLeft != null)
                {
                    yield return TopLeft;
                }
                if (TopRight != null)
                {
                    yield return TopRight;
                }
                if (BottomRight != null)
                {
                    yield return BottomRight;
                }
                if (BottomLeft != null)
                {
                    yield return BottomLeft;
                }
            }
        }

        /// <summary>
        /// 初始化<see cref="QuadTile"/>
        /// </summary>
        /// <param name="topLeft">左上角瓦片</param>
        /// <param name="topRight">右上角瓦片</param>
        /// <param name="bottomRight">右下角瓦片</param>
        /// <param name="bottomLeft">左下角瓦片</param>
        public QuadTile(TileInfo? topLeft, TileInfo? topRight, TileInfo? bottomRight, TileInfo? bottomLeft)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
        }
        /// <summary>
        /// 根据瓦片信息集合，计算相邻瓦片集合
        /// </summary>
        /// <param name="tileInfos">瓦片信息集合</param>
        /// <returns>相邻瓦片集合</returns>
        public static List<QuadTile> GetQuadTiles(IEnumerable<TileInfo> tileInfos)
        {
            var ret = new List<QuadTile>();
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
                            if (quadTile.TopLeft?.Index.Level == tileIndex.Level && quadTile.TopLeft.Index.Row == tileIndex.Row && quadTile.TopLeft.Index.Col == tileIndex.Col - 1)//为右上角
                            {
                                quadTile.TopRight = tileInfo;
                                quadTileIsExist = true;
                            }
                            else if (quadTile.TopLeft?.Index.Level == tileIndex.Level && quadTile.TopLeft.Index.Row == tileIndex.Row - 1 && quadTile.TopLeft.Index.Col == tileIndex.Col)//为左下角
                            {
                                quadTile.BottomLeft = tileInfo;
                                quadTileIsExist = true;
                            }
                            else if (quadTile.TopLeft?.Index.Level == tileIndex.Level && quadTile.TopLeft.Index.Row == tileIndex.Row - 1 && quadTile.TopLeft.Index.Col == tileIndex.Col - 1)//为右下角
                            {
                                quadTile.BottomLeft = tileInfo;
                                quadTileIsExist = true;
                            }
                        }
                        if (!quadTileIsExist)
                        {
                            var newQuadTile = new QuadTile(tileInfo, null, null, null);
                            ret.Add(newQuadTile);
                        }
                    }
                }
            }
            return ret;
        }
    }
}
