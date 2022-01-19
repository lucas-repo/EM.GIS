using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 中心点信息
    /// </summary>
    public class CenterPointInfo
    {
        /// <summary>
        /// 要素id
        /// </summary>
        public long Fid { get; set; }
        /// <summary>
        /// 几何体
        /// </summary>
        public Geometry Geometry { get; set; }
        /// <summary>
        /// 周围的要素集合
        /// </summary>
        public List<Feature> Features { get; }
        public CenterPointInfo(long fid, Geometry geometry)
        {
            Fid=fid;
            Geometry=geometry;
            Features=new List<Feature>();
        }
        public CenterPointInfo(long fid, Geometry geometry,IEnumerable<Feature> features)
        {
            Fid=fid;
            Geometry=geometry;
            Features=new List<Feature>(features);
        }
    }
}
