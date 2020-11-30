namespace EM.GIS.Symbology
{
    public class Break
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Break"/> class.
        /// </summary>
        public Break()
        {
            Name = string.Empty;
            Maximum = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Break"/> class.
        /// </summary>
        /// <param name="name">The string name for the break</param>
        public Break(string name)
        {
            Name = name;
            Maximum = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a double value for the maximum value for the break.
        /// </summary>
        public double? Maximum { get; set; }

        /// <summary>
        /// Gets or sets the string name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}