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
    /// 计算简单中心点视图模型
    /// </summary>
    public class ComputeSimpleCenterPointViewModel : ViewModel<ComputeSimpleCenterPointControl>, IDisposable, IReportable
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
        public ComputeSimpleCenterPointViewModel(ComputeSimpleCenterPointControl t) : base(t)
        {
            ExcuteCmd=new DelegateCommand(Excute, CanExcute);
            SelectPathCmd=new DelegateCommand<string>(SelectPath);
            Fields=new ObservableCollection<CheckableFieldInfo>();
            PropertyChanged+=ComputeCenterPointViewModel_PropertyChanged;
            t.Loaded+=T_Loaded;
        }

        private void T_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(View);
            window.Closed-=Window_Closed;
            window.Closed+=Window_Closed;
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            Dispose();
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
                case nameof(MaskPath):
                    SetOpenPath(ref path);
                    MaskPath=path;
                    break;
            }
        }
        private DataSource _maskDataSource;
        /// <summary>
        /// 叠加数据源
        /// </summary>
        private DataSource? MaskDataSource
        {
            get
            {
                if (_maskDataSource==null&&File.Exists(MaskPath))
                {
                    _maskDataSource= Ogr.Open(MaskPath, 0);
                }
                return _maskDataSource;
            }
            set
            {
                if (_maskDataSource!=value)
                {
                    if (_maskDataSource!=null)
                    {
                        _maskDataSource.Dispose();
                    }
                    _maskDataSource = value;
                }
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
                    if (e.PropertyName==nameof(MaskPath))
                    {
                        Fields.Clear();
                        if (MaskDataSource!=null)
                        {
                            var layer = MaskDataSource.GetLayerByIndex(0);
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
                    if (e.PropertyName==nameof(SrcPath))
                    {
                        if (string.IsNullOrEmpty(DestPath))
                        {
                            if (File.Exists(SrcPath))
                            {
                                var directory = Path.GetDirectoryName(SrcPath);
                                if (directory!=null)
                                {
                                    var name = Path.GetFileNameWithoutExtension(SrcPath);
                                    DestPath=Path.Combine(directory, $"{name}_中心点.shp");
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private bool CanExcute()
        {
            return File.Exists(SrcPath)&&!string.IsNullOrEmpty(DestPath);
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
                 var srcLayer = srcDataSource.GetLayerByIndex(0);
                 if (srcLayer==null)
                 {
                     ShowMessage($"{nameof(SrcPath)}没有图层数据");
                     return;
                 }
                 if (srcLayer.GetGeomType()!= wkbGeometryType.wkbPolygon)
                 {
                     ShowMessage($"{nameof(SrcPath)}不是面图层");
                     return;
                 }
                 var srcFeatureCount = srcLayer.GetFeatureCount(1);
                 if (srcFeatureCount==0)
                 {
                     ShowMessage($"{nameof(SrcPath)}没有数据");
                     return;
                 }
                 var maskLayer = MaskDataSource?.GetLayerByIndex(0);
                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     IsFree=false;
                 });

                 ProgressAction?.Invoke("计算相交要素中...", 5);
                 Stopwatch sw = new Stopwatch();
                 sw.Start();
                 using Driver driver = srcDataSource.GetDriver();

                 //创建中心点数据
                 var srcSpatialReference = srcLayer.GetSpatialRef();
                 using var destPointDataSource = driver.CreateDataSourceUTF8(DestPath, null);
                 var options = DriverExtensions.GetOptionsWithUTF8(null);
                 using var destPointLayer = destPointDataSource.CreateLayer(Path.GetFileNameWithoutExtension(DestPath), srcSpatialReference, wkbGeometryType.wkbPoint, options);//创建中心点图层

                 #region 添加选择的字段
                 var checkedFields = Fields.Where(x => x.IsChecked).ToList();//已选择的字段集合
                 foreach (var item in checkedFields)
                 {
                     var ret = destPointLayer.CreateField(item.FieldDefn, 1);
                 }
                 #endregion

                 ProgressAction?.Invoke("开始计算...", 10);
                 var destFeatureDefn = destPointLayer.GetLayerDefn();
                 for (int i = 0; i < srcFeatureCount; i++)
                 {
                     using var srcFt = srcLayer.GetFeature(i);
                     var centerGeo = srcFt.GetGeometryRef().Centroid();
                     var destFt = new Feature(destFeatureDefn);
                     destFt.SetGeometry(centerGeo);
                     var ret = destPointLayer.CreateFeature(destFt);
                     //foreach (var copyField in checkedFields)
                     //{
                     //    destFt.SetField(destFt, copyField.FieldName);
                     //}
                 }

                 ProgressAction?.Invoke("写入要素中", 90);
                 destPointDataSource.FlushCache();

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

        public void Dispose()
        {
            if (MaskDataSource!=null)
            {
                MaskDataSource.Dispose();
                MaskDataSource = null;
            }
        }
    }
}
