namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 绘制扩展类
    /// </summary>
    public static class DrawingExtensions
    {
        /// <summary>
        /// 将画刷转为颜色
        /// </summary>
        /// <param name="brush">画刷</param>
        /// <returns>颜色</returns>
        public static System.Drawing.Color ToColor(this System.Windows.Media.Brush brush)
        {
            System.Drawing.Color ret =  System.Drawing.Color.Empty; 
            if(brush is System.Windows.Media.SolidColorBrush solidColorBrush)
            {
                var color = solidColorBrush.Color;
                ret = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
            }
            return ret;
        }
        /// <summary>
        /// 将颜色转为画刷
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static System.Windows.Media.Brush ToBrush(this System.Drawing.Color color)
        {
            var ret = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
            return ret;
        }
    }
}
