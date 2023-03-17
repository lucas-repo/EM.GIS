using EM.GIS.Data;
using EM.GIS.Symbology;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 数据集工厂类
    /// </summary>
    public static class DataSetFactoryExtentions
    {
        /// <summary>
        /// 打开多个数据集
        /// </summary>
        /// <param name="dataSetFactory">数据集工厂</param>
        /// <param name="owner">父窗体</param>
        /// <returns>多个数据集</returns>
        public static List<IDataSet> OpenFiles(this IDataSetFactory dataSetFactory,Window? owner=null)
        {
            List<IDataSet> ret = new List<IDataSet>();
            if (dataSetFactory == null)
            {
                return ret;
            }
            var ofd = new OpenFileDialog
            { 
                Multiselect = true,
                Filter = dataSetFactory.GetFilter() 
            };
            bool? dialogResult;
            if (owner == null)
            {
                dialogResult = ofd.ShowDialog();
            }
            else
            {
                dialogResult = ofd.ShowDialog(owner);
            }
            if (dialogResult == true)
            {
                foreach (var fileName in ofd.FileNames)
                {
                    string extension=Path.GetExtension(fileName);
                    foreach (var item in dataSetFactory.Drivers)
                    {
                        if (item.Extensions == extension)
                        {
                            var ds = item.Open(fileName);
                            if (ds != null)
                            {
                                ret.Add(ds);
                                break;
                            }
                        }
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 添加多个图层
        /// </summary>
        /// <param name="frame">地图框架</param>
        /// <param name="dataSetFactory">数据集工厂</param>
        /// <param name="owner">父窗体</param>
        /// <returns>已添加的图层枚举</returns>
        public static List<ILayer> AddLayers(this IFrame frame, IDataSetFactory dataSetFactory, Window? owner = null)
        {
            List<ILayer> ret = new List<ILayer>();
            if (frame != null && dataSetFactory != null)
            {
                var dataSets = dataSetFactory.OpenFiles(owner);
                if (dataSets.Count>0)
                {
                   var selectedItem=  frame.GetSelectedItems().FirstOrDefault();
                    IGroup destGroup;
                    if (selectedItem is IGroup group)
                    {
                        destGroup = group;
                    }
                    else
                    {
                        destGroup = frame;
                    }
                    foreach (var dataSet in dataSets) 
                    {
                        if (dataSet != null)
                        {
                            var layer = destGroup.Children.AddLayer(dataSet);
                            if (layer != null)
                            {
                                ret.Add(layer);
                                layer.Frame = frame;
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}
