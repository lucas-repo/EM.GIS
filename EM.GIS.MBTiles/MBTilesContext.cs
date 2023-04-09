
using EM.SQLites;
using System.Reflection;
using System.Text;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 瓦片上下文
    /// </summary>
    public class MBTilesContext : SQLiteContext
    {
        public sqlite
        public MBTilesContext(string filename) : base(filename)
        {
        }
    }
}
