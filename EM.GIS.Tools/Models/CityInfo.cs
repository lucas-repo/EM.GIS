using EM.Bases;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 城市信息
    /// </summary>
    public class CityInfo:TreeItem
    {
        /// <summary>
        /// 行政区划编号
        /// </summary>
        public int Adcode { get;  }

        /// <summary>
        /// 实例化<see cref="CityInfo"/>
        /// </summary>
        /// <param name="adcode">行政区划编号</param>
        /// <param name="name">名称</param>
        public CityInfo(int adcode, string name)
        {
            Adcode = adcode;
            Text = name;
        }

        private int _childrenNum;
        /// <summary>
        /// 下属城市个数
        /// </summary>
        public int ChildrenNum
        {
            get { return _childrenNum; }
            set { SetProperty(ref _childrenNum, value); }
        }

    }
}
