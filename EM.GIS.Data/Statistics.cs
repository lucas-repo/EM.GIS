namespace EM.GIS.Data
{
    /// <summary>
    /// 统计
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// 最大值
        /// </summary>
        public double Maximum { get; set; } = double.NaN;
        /// <summary>
        /// 平均值
        /// </summary>
        public double Mean { get; set; } = double.NaN;
        /// <summary>
        /// 最小值
        /// </summary>
        public double Minimum { get; set; } = double.NaN;
        /// <summary>
        /// 标准差
        /// </summary>
        public double StdDeviation { get; set; } = double.NaN;
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (double.IsNaN(Maximum) || double.IsNaN(Mean) || double.IsNaN(Minimum) || double.IsNaN(StdDeviation)) return true;
            return false;
        }
    }
}