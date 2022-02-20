using EM.WpfBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 城市信息
    /// </summary>
    public class CityInfo:TreeItem<int>
    {
        private int _childrenNum;

        public CityInfo(int value) : base(value)
        {
        }

        public CityInfo(int value, string text) : base(value, text)
        {
        }

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
