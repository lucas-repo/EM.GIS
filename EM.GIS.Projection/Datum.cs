using EM.Bases;

namespace EM.GIS.Projections
{
    /// <summary>
    /// 基准面
    /// </summary>
    public class Datum: BaseCopy
    {
        #region Properties

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 椭球体
        /// </summary>
        public virtual Spheroid Spheroid { get; set; }

        /// <summary>
        /// 设置转换参数（三参或七参）
        /// </summary>
        public virtual double[] ToWGS84 { get; set; }

        /// <summary>
        /// Gets or sets the datum type, which clarifies how to perform transforms to WGS84
        /// </summary>
        public virtual DatumType DatumType { get; set; }

        /// <summary>
        /// Gets or sets the array of string nadGrid
        /// </summary>
        public virtual string[] NadGrids { get; set; }

        #endregion Properties
    }
}