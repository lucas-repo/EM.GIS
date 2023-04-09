using EM.SQLites;
using System.Data.Common;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 元数据表
    /// </summary>
    [Table("metadata")]
    public class Metadata : SQLiteTable<NameValue>
    {
        public Metadata(DbConnection connection) : base(connection, "metadata")
        {
            
        }
    }
}