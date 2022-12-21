using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Projections
{
    public interface IDatumTransform
    {
        /// <summary>
        /// Transform function
        /// </summary>
        void Transform(ProjectionInfo source, ProjectionInfo dest, double[] xy, double[] z, int startIndex, int numPoints);
    }
}
