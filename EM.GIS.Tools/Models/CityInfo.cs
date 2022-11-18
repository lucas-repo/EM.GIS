using EM.Bases;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 城市信息
    /// </summary>
    public class CityInfo:TreeItem<int>
    {
        private int _childrenNum;

        public CityInfo(int value)
        {
            Item = value;
            Text = value.ToString();
        }

        public CityInfo(int value, string text)
        {
            Item = value;
            Text = text;
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
