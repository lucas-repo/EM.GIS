using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    public static class FeatureDefnExtensions
    {
        /// <summary>
        /// 计算字段索引
        /// </summary>
        /// <param name="featureDefn">要素定义</param>
        /// <param name="fieldName">字段名</param>
        /// <returns>索引</returns>
        public static int GetFieldIndexUTF8(this FeatureDefn featureDefn, string fieldName)
        {
            int fieldIndex = 0;
            if (featureDefn!=null&&!string.IsNullOrEmpty(fieldName))
            {
                for (int i = 0; i < featureDefn.GetFieldCount(); i++)
                {
                    var fieldDefn = featureDefn.GetFieldDefn(i);
                    if (fieldDefn.GetNameUTF8()==fieldName)
                    {
                        fieldIndex=i;
                        break;
                    }
                }
            }
            return fieldIndex;
        }
    }
}
