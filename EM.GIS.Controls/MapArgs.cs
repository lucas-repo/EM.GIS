using EM.GIS.Data;
using EM.GIS.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EM.GIS.Controls
{
    public class MapArgs : EventArgs, IProj
    {
        #region  Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapArgs"/> class.
        /// </summary>
        /// <param name="bufferRectangle">The buffer rectangle.</param>
        /// <param name="bufferEnvelope">The buffer envelope.</param>
        public MapArgs(Rectangle bufferRectangle, IExtent bufferEnvelope)
        {
            Bounds = bufferRectangle;
            Extent = bufferEnvelope;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapArgs"/> class, where the device is also specified, overriding the default buffering behavior.
        /// </summary>
        /// <param name="bufferRectangle">The buffer rectangle.</param>
        /// <param name="bufferEnvelope">The buffer envelope.</param>
        /// <param name="g">The graphics object used for drawing.</param>
        public MapArgs(Rectangle bufferRectangle, IExtent bufferEnvelope, Graphics g)
        {
            Bounds = bufferRectangle;
            Extent = bufferEnvelope;
            Device = g;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an optional parameter that specifies a device to use instead of the normal buffers.
        /// </summary>
        public Graphics Device { get; }

        /// <summary>
        /// Gets the Dx
        /// </summary>
        public double Dx => Extent.Width != 0.0 ? Bounds.Width / Extent.Width : 0.0;

        /// <summary>
        /// Gets the Dy
        /// </summary>
        public double Dy => Extent.Height != 0.0 ? Bounds.Height / Extent.Height : 0.0;

        /// <summary>
        /// Gets the geographic bounds of the content of the buffer.
        /// </summary>
        public IExtent Extent { get; }

        /// <summary>
        /// Gets the rectangle dimensions of what the buffer should be in pixels
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Gets the maximum Y value
        /// </summary>
        public double MaxY => Extent.MaxY;

        /// <summary>
        /// Gets the minimum X value
        /// </summary>
        public double MinX => Extent.MinX;

        #endregion
    }
}
