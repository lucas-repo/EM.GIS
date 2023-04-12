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
    public class MBTilesSet:RasterSet
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        public MBTilesContext Context { get; }
        public MBTilesSet(string filename)
        {
            Context=new MBTilesContext(filename); 
        }
        /// <inheritdoc/>
        protected override Rectangle OnDraw(MapArgs mapArgs, Action<int>? progressAction = null, Func<bool>? cancelFunc = null, Action<Rectangle>? graphicsUpdatedAction = null, Dictionary<string, object>? options = null)
        {
            Rectangle ret=Rectangle.Empty;
            var metadataInfo = Context.Metadata.GetMetadataInfo();//测试下时间
            if (metadataInfo != null)
            {
                
            }
            return ret;
        }
    }
}
