using EM.GIS.Projections;
using EM.IOC;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.Gdals
{
    /// <summary>
    /// 投影工厂
    /// </summary>
    [Injectable(ServiceLifetime = ServiceLifetime.Singleton, ServiceType = typeof(IProjectionFactory))]
    public class GdalProjectionFactory : IProjectionFactory
    {
        /// <inheritdoc/>
        public IProjection GetProjection(int epsg)
        {
            var projection = new GdalProjection(epsg);
            return projection;
        }
    }
}
