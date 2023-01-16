namespace EM.GIS.Projections
{
    /// <summary>
    /// 角度单位
    /// </summary>
    public class AngularUnit
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 获取或设置与此单位相乘的常数以获得弧度
        /// </summary>
        public virtual double Radians { get; set; }
    }
}