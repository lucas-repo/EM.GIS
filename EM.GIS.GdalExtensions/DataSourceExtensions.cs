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
                //添加字段
                for (int i = 0; i < srcFeatureDefn.GetFieldCount(); i++)
                {
                    var srcFieldDefn = srcFeatureDefn.GetFieldDefn(i);
                    var fieldName = srcFieldDefn.GetName();
                    FieldDefn destFieldDefn = new FieldDefn(fieldName, srcFieldDefn.GetFieldType());
                    var ret = destLayer.CreateField(destFieldDefn, 1);
                    if (ret!=0)
                    {
                        Console.WriteLine($"{nameof(CreateLayer)}创建字段{fieldName}失败");
                    }
                }
            }
            return destLayer;
        }
        /// <summary>
        /// 创建字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldType">字段类型</param>
        /// <returns>成功0</returns>
        public static int CreateField(this Layer layer, string fieldName, FieldType fieldType)
        {
            FieldDefn destFieldDefn = new FieldDefn(fieldName, fieldType);
            var ret = layer.CreateField(destFieldDefn, 1);
            if (ret!=0)
            {
                Console.WriteLine($"{nameof(CreateField)}创建字段{fieldName}失败:{ret}");
            }
            return ret;
        }
    }
}
