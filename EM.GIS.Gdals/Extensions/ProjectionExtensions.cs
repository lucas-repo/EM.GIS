using EM.GIS.Projections;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    public static class ProjectionExtensions
    {
        /// <summary>
        /// 转投影信息
        /// </summary>
        /// <param name="spatialReference"></param>
        /// <returns></returns>
        public static Projection ToProjectionInfo(this SpatialReference spatialReference)
        {
            Projection projectionInfo = null;
            if (spatialReference != null)
            {
                projectionInfo = new GdalProjection(spatialReference);
            }
            return projectionInfo;
        }
    }
}
