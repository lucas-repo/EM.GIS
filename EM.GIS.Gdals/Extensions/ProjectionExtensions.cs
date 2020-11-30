using EM.GIS.Projection;
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
        public static ProjectionInfo ToProjectionInfo(this SpatialReference spatialReference)
        {
            ProjectionInfo projectionInfo = null;
            if (spatialReference != null)
            {
                projectionInfo = new GdalProjectionInfo(spatialReference);
            }
            return projectionInfo;
        }
    }
}
