using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// Interface for ImageLayer.
    /// </summary>
    public interface IImageLayer : ILayer
    {
        /// <summary>
        /// Gets or sets a class that has some basic parameters that control how the image layer
        /// is drawn.
        /// </summary>
        IImageSymbolizer Symbolizer { get; set; }

        ///// <summary>
        ///// Gets or sets the dataset specifically as an IImageData object
        ///// </summary>
        //new IImageData DataSet { get; set; }

        ///// <summary>
        ///// Gets or sets the image being drawn by this layer
        ///// </summary>
        //IImageData Image { get; set; }
    }
}
