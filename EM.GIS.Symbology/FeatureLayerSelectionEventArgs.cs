namespace EM.GIS.Symbology
{
    /// <summary>
    /// Event args for events that need a feature layer selection.
    /// </summary>
    public class FeatureLayerSelectionEventArgs : FeatureLayerEventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureLayerSelectionEventArgs"/> class.
        /// </summary>
        /// <param name="fl">The feature layer.</param>
        /// <param name="selection">The selection.</param>
        public FeatureLayerSelectionEventArgs(IFeatureLayer fl, ISelection selection)
            : base(fl)
        {
            Selection = selection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of changed features.
        /// </summary>
        public ISelection Selection { get; protected set; }

        #endregion
    }
}