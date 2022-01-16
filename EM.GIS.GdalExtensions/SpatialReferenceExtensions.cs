using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class SpatialReferenceExtensions
    {
        /// <summary>
        /// 获取或更新空间参考授权名称及编号，一般名称为“EPSG”
        /// </summary>
        /// <param name="spatialReference">空间参考</param>
        /// <returns>名称及编号</returns>
        public static (string AuthorityName, string AuthorityCode) GetOrUpdateAuthorityNameAndCode(this SpatialReference spatialReference)
        {
            if (spatialReference.GetAuthorityName(null)==null||spatialReference.GetAuthorityCode(null)==null)
            {
                var ogrErr = spatialReference.AutoIdentifyEPSG();
                switch (ogrErr)
                {
                    case 1:
                        Console.WriteLine("不支持的坐标系");
                        break;
                }
            }
            var authorityName = spatialReference.GetAuthorityName(null);
            var authorityCode = spatialReference.GetAuthorityCode(null);
            return (authorityName, authorityCode);
        }
    }
}
