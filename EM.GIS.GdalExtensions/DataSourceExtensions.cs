using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class DataSourceExtensions
    {
        /// <summary>
        /// 创建与指定图层相同结构的图层
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="name">图层名称</param>
        /// <param name="schemaLayer">指定结构的图层</param>
        /// <param name="options">可选项</param>
        /// <returns>图层</returns>
        public static Layer CreateLayer(this DataSource dataSource, string name, Layer schemaLayer, string[] options = null)
        {
            Layer destLayer = null;
            if (dataSource!=null&&!string.IsNullOrEmpty(name)&&schemaLayer!=null)
            {
                var srcFeatureDefn = schemaLayer.GetLayerDefn();
                destLayer = dataSource.CreateLayer(name, schemaLayer.GetSpatialRef(), schemaLayer.GetGeomType(), options);
                var destFeatureDefn = destLayer.GetLayerDefn();
                //添加字段
                for (int i = 0; i < srcFeatureDefn.GetFieldCount(); i++)
                {
                    var srcFieldDefn = srcFeatureDefn.GetFieldDefn(i);
                    FieldDefn destFieldDefn = new FieldDefn(srcFieldDefn.GetName(), srcFieldDefn.GetFieldType());
                    destFeatureDefn.AddFieldDefn(destFieldDefn);
                }
            }
            return destLayer;
        }
    }
}
