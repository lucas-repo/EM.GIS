using EM.WpfBase;
using Microsoft.Win32;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EM.GIS.GdalExtensions;
using System.Threading;
using System.Diagnostics;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 计算中心点视图模型
    /// </summary>
    public class ComputeCenterPointViewModel : ViewModel<ComputeCenterPointControl>
    {
        private string _srcPath;
        /// <summary>
        /// 原数据路径
        /// </summary>
        public string SrcPath
        {
            get { return _srcPath; }
            set { SetProperty(ref _srcPath, value); }
        }
        private string _destPath;
        /// <summary>
        /// 目标数据路径
        /// </summary>
        public string DestPath
        {
            get { return _destPath; }
            set { SetProperty(ref _destPath, value); }
        }
        private string _maskPath;
        /// <summary>
        /// 叠加数据路径
        /// </summary>
        public string MaskPath
        {
            get { return _maskPath; }
            set { SetProperty(ref _maskPath, value); }
        }
        private uint _iterationCount = 1;
        /// <summary>
        /// 迭代次数
        /// </summary>
        public uint IterationCount
        {
            get { return _iterationCount; }
            set
            {
                if (value>0)
                {
                    SetProperty(ref _iterationCount, value);
                }
            }
        }
        private bool _isFree = true;
        /// <summary>
        /// 是否空闲
        /// </summary>
        public bool IsFree
        {
            get { return _isFree; }
            set { SetProperty(ref _isFree, value); }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        public DelegateCommand ExcuteCmd { get; }
        /// <summary>
        /// 选择目录命令
        /// </summary>
        public DelegateCommand<string> SelectPathCmd { get; }
        /// <summary>
        /// 进度委托
        /// </summary>
        public Action<string, int> ProgressAction { get; set; }
        public ComputeCenterPointViewModel(ComputeCenterPointControl t) : base(t)
        {
            ExcuteCmd=new DelegateCommand(Excute, CanExcute);
            SelectPathCmd=new DelegateCommand<string>(SelectPath);
            PropertyChanged+=ComputeCenterPointViewModel_PropertyChanged;
        }
        private void SelectPath(string? pathName)
        {
            OpenFileDialog openFileDialog;
            switch (pathName)
            {
                case nameof(SrcPath):
                    openFileDialog = new OpenFileDialog()
                    {
                        Title="选择要转换的要素",
                        Filter="*.shp|*.shp"
                    };
                    if (openFileDialog.ShowDialog()==true)
                    {
                        SrcPath=openFileDialog.FileName;
                    }
                    break;
                case nameof(DestPath):
                    SaveFileDialog saveFileDialog = new SaveFileDialog()
                    {
                        Title="选择要保存的位置",
                        Filter="*.shp|*.shp"
                    };
                    if (saveFileDialog.ShowDialog()==true)
                    {
                        DestPath=saveFileDialog.FileName;
                    }
                    break;
                case nameof(MaskPath):
                    openFileDialog = new OpenFileDialog()
                    {
                        Title="选择要转换的要素",
                        Filter="*.shp|*.shp"
                    };
                    if (openFileDialog.ShowDialog()==true)
                    {
                        MaskPath=openFileDialog.FileName;
                    }
                    break;
            }
        }
        private void ComputeCenterPointViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SrcPath):
                case nameof(DestPath):
                case nameof(MaskPath):
                    ExcuteCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanExcute()
        {
            return File.Exists(SrcPath)&&File.Exists(MaskPath)&&DestPath!=SrcPath&&DestPath!=MaskPath;
        }
        void ShowMessage(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(Window.GetWindow(View), text);
                IsFree=true;
            });
        }
        private void Excute()
        {
            var task = Task.Run(() =>
             {
                 using DataSource srcDataSource = Ogr.Open(SrcPath, 0);
                 if (srcDataSource==null)
                 {
                     ShowMessage($"无法打开{SrcPath}");
                     return;
                 }
                 using var srcLayer = srcDataSource.GetLayerByIndex(0);
                 if (srcLayer==null)
                 {
                     ShowMessage($"{SrcPath}没有图层数据");
                     return;
                 }
                 var srcFeatureCount = srcLayer.GetFeatureCount(1);
                 if (srcFeatureCount==0)
                 {
                     ShowMessage($"{SrcPath}没有数据");
                     return;
                 }
                 using DataSource maskDataSource = Ogr.Open(MaskPath, 0);
                 if (maskDataSource==null)
                 {
                     ShowMessage($"无法打开{MaskPath}");
                     return;
                 }
                 using var maskLayer = maskDataSource.GetLayerByIndex(0);
                 if (maskLayer==null)
                 {
                     ShowMessage($"{MaskPath}没有图层数据");
                     return;
                 }
                 var maskFeatureCount = maskLayer.GetFeatureCount(1);
                 if (maskFeatureCount==0)
                 {
                     ShowMessage($"{MaskPath}没有数据");
                     return;
                 }
                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     IsFree=false;
                 });

                 ProgressAction?.Invoke("计算相交要素中...", 5);
                 Stopwatch sw = new Stopwatch();
                 sw.Start();
                 using Driver driver = srcDataSource.GetDriver();
                 var srcSpatialReference = srcLayer.GetSpatialRef();
                 using var destDataSource = driver.CreateDataSource(DestPath, null);
                 var destName = Path.GetFileNameWithoutExtension(DestPath);
                 using var destLayer = destDataSource.CreateLayer(destName, srcSpatialReference, wkbGeometryType.wkbPoint, null);//创建图层
                #region 根据叠加图层随机取相交的原始要素中心点信息集合
                var centerPointInfos = new List<CenterPointInfo>();//中心点信息集合
                for (long i = 0; i < maskFeatureCount; i++)
                 {
                     using var maskFeature = maskLayer.GetFeature(i);
                     var maskGeometry = maskFeature.GetGeometryRef();//返回的几何图形仍归容器所有，不应修改
                    srcLayer.SetSpatialFilter(maskGeometry);
                     using var srcFeature = srcLayer.GetNextFeature();
                     if (srcFeature!=null)
                     {
                         var centerPointInfo = new CenterPointInfo(srcFeature.GetFID(), srcFeature.GetGeometryRef().Centroid());
                         centerPointInfos.Add(centerPointInfo);
                     }
                 }
                 ProgressAction?.Invoke("开始迭代...", 10);
                #endregion
                //开始迭代
                if (centerPointInfos.Count>0)
                 {
                     var progressIncrement = 80.0/IterationCount/srcFeatureCount;
                     int oldProgress = 10;
                     for (int i = 0; i < IterationCount; i++)
                     {
                        #region 释放中心点附近的要素
                        foreach (var item in centerPointInfos)
                         {
                             item.Features.ForEach(x => x.Dispose());
                             item.Features.Clear();
                         }
                        #endregion

                        List<Feature> features = new List<Feature>();
                        #region 计算离中心点较近的要素
                        for (int srcFeatureIndex = 0; srcFeatureIndex < srcFeatureCount; srcFeatureIndex++)
                         {
                             int newProgress = 10+(int)((srcFeatureCount*i+srcFeatureIndex)*progressIncrement);
                             if (newProgress>oldProgress)
                             {
                                 oldProgress=newProgress;
                                 ProgressAction?.Invoke($"第{i}次迭代，计算中心点周围要素中...", oldProgress);
                             }
                             var tmpFeature = srcLayer.GetFeature(srcFeatureIndex);
                             double minDistance = double.MaxValue;
                             CenterPointInfo? minDistanceCenterPointInfo = null;
                             foreach (var item in centerPointInfos)
                             {
                                 double distance = item.Geometry.Distance(tmpFeature.GetGeometryRef());
                                 if (distance<minDistance)
                                 {
                                     minDistance=distance;
                                     minDistanceCenterPointInfo =item;
                                 }
                             }
                             if (minDistanceCenterPointInfo!=null)
                             {
                                 minDistanceCenterPointInfo.Features.Add(tmpFeature);
                             }
                         }
                        #endregion

                        //ProgressAction?.Invoke($"第{i}次迭代，计算中心点中...", 10+(int)(srcFeatureCount*i+srcFeatureIndex));
                        #region 重新计算中心点
                        foreach (var item in centerPointInfos)
                         {
                             var newCentroid = item.Features.Select(x => x.GetGeometryRef()).GetCentroid();
                             var oldCentrolid = item.Geometry;
                             item.Geometry=newCentroid;
                             oldCentrolid.Dispose();
                         }
                        #endregion
                    }
                     ProgressAction?.Invoke("写入要素中", 90);
                    #region 添加要素至目标图层
                    var featureDefn = destLayer.GetLayerDefn();
                     foreach (var item in centerPointInfos)
                     {
                         if (item!=null)
                         {
                             Feature feature = new(featureDefn);
                             feature.SetGeometry(item.Geometry);
                             destLayer.CreateFeature(feature);
                             item.Features.ForEach(x => x.Dispose());
                         }
                     }
                     destDataSource.FlushCache();
                    #endregion
                }
                 ProgressAction?.Invoke("", 100);
                 sw.Stop();
                 TimeSpan timeSpan = new TimeSpan(sw.ElapsedTicks);
                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     MessageBox.Show(Window.GetWindow(View), $"完成时间{timeSpan}");
                     IsFree=true;
                 });
             });
        }
    }
}
