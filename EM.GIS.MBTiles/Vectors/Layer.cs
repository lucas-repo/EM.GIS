using System;
using System.Collections.Generic;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 图层
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// 图层
        /// </summary>
        public string layer { get; set; }=string.Empty;
        /// <summary>
        /// 要素个数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 几何类型
        /// </summary>
        public Geometry geometry { get; set; }
        /// <summary>
        /// 属性个数
        /// </summary>
        public int attributeCount { get; set; }
        /// <summary>
        /// 属性集合
        /// </summary>
        public List<Attribute<object>> attributes { get; }=new List<Attribute<object>>();
    }
}