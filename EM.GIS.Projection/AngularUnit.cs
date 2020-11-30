namespace EM.GIS.Projection
{
    /// <summary>
    /// 角度单位
    /// </summary>
    public class AngularUnit
    {
        /// <summary>
        /// Gets or sets the name of this
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the constant to multiply against this unit to get radians.
        /// </summary>
        public virtual double Radians { get; set; }
    }
}