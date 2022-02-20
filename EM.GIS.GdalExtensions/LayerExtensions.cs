using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EM.GIS.GdalExtensions
{
    /// <summary>
    /// 图层扩展方法
    /// </summary>
    public static class LayerExtensions
    {
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>名称</returns>
        [DllImport(FeatureExtensions.GdalDllName, CallingConvention = CallingConvention.Cdecl)]
        public extern static IntPtr OGR_L_GetName(HandleRef layer);
        /// <summary>
        /// 获取范围
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>范围</returns>
        public static Envelope GetEnvelope(this Layer layer)
        {
            Envelope envelope = null;
            if (layer != null)
            {
                envelope = new Envelope();
                var ret = layer.GetExtent(envelope, 1);
            }
            return envelope;
        }
        /// <summary>
        /// 创建多个字段
        /// </summary>
        /// <param name="layer">图层</param>
        /// <param name="fieldDefns">字段集合</param>
        public static void CreateFields(this Layer layer, IEnumerable<FieldDefn> fieldDefns)
        {
            if (layer!=null&&fieldDefns!=null)
            {
                foreach (var fieldDefn in fieldDefns)
                {
                    if (fieldDefn!=null)
                    {
                        var ret = layer.CreateField(fieldDefn, 1);
                        if (ret!=0)
                        {
                            Console.WriteLine($"{nameof(CreateFields)}创建字段{fieldDefn.GetName()}失败");
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="layer">图层</param>
        /// <returns>名称</returns>
        public static string GetNameUTF8(this Layer layer)
        {
            var layerRef = Layer.getCPtr(layer);
            IntPtr strPtr = OGR_L_GetName(layerRef);
            string value = strPtr.IntPtrTostring(Encoding.UTF8);
            return value;
        }
        /// <summary>
        /// 复制图层数据
        /// </summary>
        /// <param name="destLayer">目标图层</param>
        /// <param name="srcLayer">源图层</param>
        public static void CopyLayer(this Layer destLayer, Layer srcLayer)
        {
            if (destLayer==null||srcLayer==null||destLayer==srcLayer)
            {
                return;
            }
            var srcFeatureCount = srcLayer.GetFeatureCount(1);
            if (srcFeatureCount>0)
            {
                var srcFeatureDefn = srcLayer.GetLayerDefn();
                var destFeatureDefn = destLayer.GetLayerDefn();
                var destFieldCount = destFeatureDefn.GetFieldCount();
                Dictionary<int, int> fieldIndexDic = new Dictionary<int, int>();
                for (long i = 0; i < srcFeatureCount; i++)
                {
                    var srcFeature = srcLayer.GetFeature(i);
                    var destFeature = new Feature(destFeatureDefn);
                    var destGeometry = srcFeature.GetGeometryRef().Clone();
                    destFeature.SetGeometry(destGeometry);
                    for (int destFieldIndex = 0; destFieldIndex <destFieldCount; destFieldIndex++)
                    {
                        int srcFieldIndex;
                        if (fieldIndexDic.ContainsKey(destFieldIndex))
                        {
                            srcFieldIndex=fieldIndexDic[destFieldIndex];
                        }
                        else
                        {
                            var destFieldName = destFeatureDefn.GetFieldDefn(destFieldIndex).GetNameUTF8();
                            srcFieldIndex = srcFeatureDefn.GetFieldIndexUTF8(destFieldName);
                            fieldIndexDic[destFieldIndex]=srcFieldIndex;
                        }
                        destFeature.SetField(destFieldIndex, srcFeature, srcFieldIndex);
                    }
                    int ret = destLayer.CreateFeature(destFeature);
                }
            }
        }
    }
}
