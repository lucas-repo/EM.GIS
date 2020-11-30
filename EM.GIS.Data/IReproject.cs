using EM.GIS.Projection;

namespace EM.GIS.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReproject
    {
        /// <summary>
        /// Gets a value indicating whether the DotSpatial.Projections assembly is loaded. If
        /// not, this returns false, and neither ProjectionInfo nor the Reproject() method should
        /// be used.
        /// </summary>
        /// <returns>Boolean, true if the value can reproject.</returns>
        bool CanReproject { get; }

        /// <summary>
        /// Gets or sets the projection information for this dataset
        /// </summary>
        ProjectionInfo Projection { get; set; }

        /// <summary>
        /// Reprojects all of the in-ram vertices of vectors, or else this
        /// simply updates the "Bounds" of image and raster objects
        /// This will also update the projection to be the specified projection.
        /// </summary>
        /// <param name="targetProjection">
        /// The projection information to reproject the coordinates to.
        /// </param>
        void Reproject(ProjectionInfo targetProjection);
    }
}