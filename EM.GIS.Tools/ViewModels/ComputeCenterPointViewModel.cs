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
using System.Collections.ObjectModel;

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
        /// <summary>
        /// 所有字段
        /// </summary>
        public ObservableCollection<CheckableFieldInfo> Fields { get; }
        private string _destPointPath;
        /// <summary>
        /// 目标数据路径
        /// </summary>
        public string DestPointPath
        {
            get { return _destPointPath; }
            set { SetProperty(ref _destPointPath, value); }
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
            Fields=new ObservableCollection<CheckableFieldInfo>();
            PropertyChanged+=ComputeCenterPointViewModel_PropertyChanged;
        }
        private OpenFileDialog OpenShpDialog()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Title="选择要转换的要素",
                Filter="*.shp|*.shp"
            };
            return openFileDialog;
        }
        private SaveFileDialog SaveShpDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title="选择要保存的位置",
                Filter="*.shp|*.shp"
            };
            return saveFileDialog;
        }
        private void SetOpenPath(ref string path)
        {
            var openFileDialog = OpenShpDialog();
            if (openFileDialog.ShowDialog()==true)
            {
                path=openFileDialog.FileName;
            }
        }
        private void SetSavePath(ref string path)
        {
            var saveFileDialog = SaveShpDialog();
            if (saveFileDialog.ShowDialog()==true)
            {
                path=saveFileDialog.FileName;
            }
        }
        private void SetSavePath(string path)
        {
            var saveFileDialog = SaveShpDialog();
            if (saveFileDialog.ShowDialog()==true)
            {
                path=saveFileDialog.FileName;
            }
        }
        private void SelectPath(string? pathName)
        {
            string path = string.Empty;
            switch (pathName)
            {
                case nameof(SrcPath):
                    SetOpenPath(ref path);
                    SrcPath=path;
                    break;
                case nameof(DestPath):
                    SetSavePath(ref path);
                    DestPath=path;
                    break;
                case nameof(DestPointPath):
                    SetSavePath(ref path);
                    DestPointPath=path;
                    break;
                case nameof(MaskPath):
                    SetOpenPath(ref path);
                    MaskPath=path;
                    break;
            }
        }
        private void ComputeCenterPointViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SrcPath):
                case nameof(DestPointPath):
                case nameof(MaskPath):
                    ExcuteCmd.RaiseCanExecuteChanged();
                    if (e.PropertyName==nameof(MaskPath))
                    {
                        Fields.Clear();
                        using DataSource dataSource = Ogr.Open(MaskPath, 0);
                        if (dataSource!=null)
                        {
                            using var layer = dataSource.GetLayerByIndex(0);
                            if (layer!=null)
                            {
                                var featureDefn = layer.GetLayerDefn();
                                for (int i = 0; i < featureDefn.GetFieldCount(); i++)
                                {
                                    var fieldDefn = featureDefn.GetFieldDefn(i);
                                    Fields.Add(new CheckableFieldInfo(fieldDefn));
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private bool CanExcute()
        {
            return File.Exists(SrcPath)&&!string.IsNullOrEmpty(DestPath)&&File.Exists(MaskPath)&&!string.IsNullOrEmpty(DestPointPath);
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
                     ShowMessage($"{nameof(SrcPath)}没有图层数据");
                     return;
                 }
                 var srcFeatureCount = srcLayer.GetFeatureCount(1);
                 if (srcFeatureCount==0)
                 {
                     ShowMessage($"{nameof(SrcPath)}没有数据");
                     return;
                 }
                 using DataSource maskDataSource = Ogr.Open(MaskPath, 0);
                 if (maskDataSource==null)
                 {
                     ShowMessage($"无法打开{nameof(MaskPath)}");
                     return;
                 }
                 using var maskLayer = maskDataSource.GetLayerByIndex(0);
                 if (maskLayer==null)
                 {
                     ShowMessage($"{nameof(MaskPath)}没有图层数据");
                     return;
                 }
                 var maskFeatureCount = maskLayer.GetFeatureCount(1);
                 if (maskFeatureCount==0)
                 {
                     ShowMessage($"{nameof(MaskPath)}没有数据");
                     return;
                 }
                 if (string.IsNullOrEmpty(DestPath))
                 {
                     ShowMessage($"{nameof(DestPath)}不能为空");
                     return;
                 }
                 if (string.IsNullOrEmpty(DestPointPath))
                 {
                     ShowMessage($"{nameof(DestPointPath)}不能为空");
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
                 //创建目标数据
                 using var destDataSource = driver.CopyDataSource(srcDataSource, DestPath, null);//将源数据复制到目标数据
                 using var destLayer = destDataSource.GetLayerByIndex(0);

                 //创建中心点数据
                 var srcSpatialReference = srcLayer.GetSpatialRef();
                 using var destPointDataSource = driver.CreateDataSource(DestPointPath, null);
                 using var destPointLayer = destPointDataSource.CreateLayer(Path.GetFileNameWithoutExtension(DestPointPath), srcSpatialReference, wkbGeometryType.wkbPoint, null);//创建中心点图层
                 
                 #region 添加选择的字段
                 var checkedFields = Fields.Where(x => x.IsChecked).ToList();//已选择的字段集合
                 foreach (var item in checkedFields)
                 {
                     var ret0 = destLayer.CreateField(item.FieldName, item.FieldType);
                     var ret1 = destPointLayer.CreateField(item.FieldName, item.FieldType);
                 }
                 #endregion

                 #region 根据叠加图层随机取相交的原始要素中心点信息集合
                 var centerPointInfos = new List<CenterPointInfo>();//中心点信息集合
                 for (long i = 0; i < maskFeatureCount; i++)
                 {
                     using var maskFeature = maskLayer.GetFeature(i);
                     var maskGeometry = maskFeature.GetGeometryRef();//返回的几何图形仍归容器所有，不应修改
                     destLayer.SetSpatialFilter(maskGeometry);
                     using var srcFeature = destLayer.GetNextFeature();
                     if (srcFeature!=null)
                     {
                         var centerPointInfo = new CenterPointInfo(maskFeature.GetFID(), srcFeature.GetGeometryRef().Centroid()); 
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
                             var tmpFeature = destLayer.GetFeature(srcFeatureIndex);
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
                     var destPointFeatureDefn = destPointLayer.GetLayerDefn();
                     foreach (var item in centerPointInfos)
                     {
                         if (item!=null)
                         {
                             using var maskFeature = maskLayer.GetFeature(item.Fid);
                             //目标图层创建要素
                             foreach (var feature in item.Features)
                             {
                                 var featureCopy = feature.Clone();
                                 foreach (var copyField in checkedFields)
                                 {
                                     featureCopy.SetField(maskFeature, copyField.FieldName);
                                 }
                                 destLayer.CreateFeature(featureCopy);
                             }
                             //中心点图层创建要素
                             Feature destPointFeature = new(destPointFeatureDefn);
                             destPointFeature.SetGeometry(item.Geometry);
                             destPointLayer.CreateFeature(destPointFeature);
                             foreach (var copyField in checkedFields)
                             {
                                 destPointFeature.SetField(maskFeature, copyField.FieldName);
                             }
                             item.Features.ForEach(x => x.Dispose());
                         }
                     }
                     destDataSource.FlushCache();
                     destPointDataSource.FlushCache();
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
