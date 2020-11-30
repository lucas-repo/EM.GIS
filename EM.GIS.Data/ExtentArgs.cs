using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Data
{
    /// <summary>
    /// An EventArgs class for passing around an extent.
    /// </summary>
    public class ExtentArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtentArgs"/> class.
        /// </summary>
        /// <param name="value">The value for this event.</param>
        public ExtentArgs(IExtent value)
        {
            Extent = value;
        }

        /// <summary>
        /// Gets or sets the Extents for this event.
        /// </summary>
        public IExtent Extent { get; protected set; }
    }
}
