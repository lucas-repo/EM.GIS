namespace EM.GIS.Projection
{
    /// <summary>
    /// 基准面
    /// </summary>
    public class Datum: BaseCopy
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the datum defining the spherical characteristics of the model of the earth
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// The spheroid of the earth, defining the maximal radius and the flattening factor
        /// </summary>
        public virtual Spheroid Spheroid { get; set; }

        /// <summary>
        /// Gets or sets the set of double parameters, (this can either be 3 or 7 parameters)
        /// used to transform this
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