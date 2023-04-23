using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Attribute<T>
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string attribute { get; set; } = string.Empty;
        /// <summary>
        /// 枚举值个数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; } = string.Empty;
        /// <summary>
        /// 值集合
        /// </summary>
        public List<T> values { get; }= new List<T>();
        /// <summary>
        /// 最小值
        /// </summary>
        public double min { get; set; } = double.NaN;
        /// <summary>
        /// 最大值
        /// </summary>
        public double max { get; set; } = double.NaN;
    }
}
