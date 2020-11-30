using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// Event args for events that need a feature layer.
    /// </summary>
    public class FeatureLayerEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureLayerEventArgs"/> class.
        /// </summary>
        /// <param name="featureLayer">FeatureLayer of the event.</param>
        public FeatureLayerEventArgs(IFeatureLayer featureLayer)
        {
            FeatureLayer = featureLayer;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the feature layer for this event.
        /// </summary>
        public IFeatureLayer FeatureLayer { get; protected set; }

        #endregion
    }
}