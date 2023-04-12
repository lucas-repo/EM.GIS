using EM.SQLites;
using System.Data.Common;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 元数据表
    /// </summary>
    public class Metadata : SQLiteTable<NameValue>
    {
        public Metadata(DbConnection connection) : base(connection, "metadata")
        {
            
        }
        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <returns></returns>
        public MetadataInfo? GetMetadataInfo()
        {
            MetadataInfo? ret=null;
            var nameValues = GetObjects();
            if (nameValues.Count > 0)
            {
                ret=new MetadataInfo(nameValues);
            }
            return ret;
        }
        /// <summary>
        /// 设置元数据信息
        /// </summary>
        /// <param name="metadataInfo"></param>
        public void SetMetadataInfo(MetadataInfo metadataInfo)
        {
            var nameValues = metadataInfo.ToNameValues();
            Update(nameValues);
        }
    }
}