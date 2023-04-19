
using EM.SQLites;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 瓦片上下文
    /// </summary>
    public class MBTilesContext : SQLiteContext
    {
        /// <summary>
        /// 元素据表
        /// </summary>
        public Metadata Metadata { get; }
        /// <summary>
        /// 瓦片表
        /// </summary>
        public Tiles Tiles { get; }
        ///// <summary>
        ///// 栅格表（可选）
        ///// </summary>
        //public SQLiteTable<Grid> Grids { get; }
        ///// <summary>
        ///// 栅格数据表（可选）
        ///// </summary>
        //public SQLiteTable<GridData> GridDatas { get; }
        public MBTilesContext(string filename) : base(filename)
        {
            Metadata = new Metadata(Connection);
            Tiles = new Tiles(Connection);
        }
        public static MBTilesContext? CreateMBTilesContext(string filename,MetadataInfo metadataInfo)
        {
            MBTilesContext? ret = null;
            if (string.IsNullOrEmpty(filename) )
            {
                return ret;
            }
            try
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
                ret = new MBTilesContext(filename);
                ret.Metadata.SetMetadataInfo(metadataInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{nameof(CreateMBTilesContext)}失败，{e}");
            }
            return ret;
        }
    }
}
