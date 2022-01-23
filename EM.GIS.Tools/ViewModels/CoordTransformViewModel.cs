using EM.GIS.CoordinateTransformation;
using EM.GIS.GdalExtensions;
using EM.WpfBase;
using Microsoft.Win32;
using OSGeo.OGR;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 坐标转换视图模型
    /// </summary>
    public class CoordTransformViewModel : WpfBase.ViewModel<CoordTransformControl>
    {
        private OffsetType _srcOffsetType;
        /// <summary>
        /// 原坐标偏移类型
        /// </summary>
        public OffsetType SrcOffsetType
        {
            get { return _srcOffsetType; }
            set { SetProperty(ref _srcOffsetType, value); }
        }
        private string _srcPath;
        /// <summary>
        /// 原文件路径
        /// </summary>
        public string SrcPath
        {
            get { return _srcPath; }
            set { SetProperty(ref _srcPath, value); }
        }
        /// <summary>
        /// 偏移类型集合
        /// </summary>
        public ObservableCollection<OffsetType> OffsetTypes { get; }
        private OffsetType _destOffsetType;
        /// <summary>
        /// 目标坐标偏移类型
        /// </summary>
        public OffsetType DestOffsetType
        {
            get { return _destOffsetType; }
            set { SetProperty(ref _destOffsetType, value); }
        }

        private string _destPath;
        /// <summary>
        /// 目标文件路径
        /// </summary>
        public string DestPath
        {
            get { return _destPath; }
            set { SetProperty(ref _destPath, value); }
        }
        /// <summary>
        /// 选择目录命令
        /// </summary>
        public DelegateCommand<string> SelectPathCmd { get; }
        /// <summary>
        /// 坐标转换命令
        /// </summary>
        public DelegateCommand TransformCmd { get; }
        public CoordTransformViewModel(CoordTransformControl t) : base(t)
        {
            var offsetTypes = Enum.GetValues(typeof(OffsetType));
            OffsetTypes=new ObservableCollection<OffsetType>();
            foreach (var item in offsetTypes)
            {
                OffsetTypes.Add((OffsetType)item);
            }
            SelectPathCmd=new DelegateCommand<string>(SelectPath);
            TransformCmd =new DelegateCommand(Transform, CanTransform);
            PropertyChanged+=CoordTransformViewModel_PropertyChanged;
        }

        private void SelectPath(string? pathName)
        {
            switch (pathName)
            {
                case nameof(SrcPath):
                    OpenFileDialog openFileDialog = new OpenFileDialog()
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
                default:
                    return;
            }
        }

        private void CoordTransformViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SrcPath):
                case nameof(DestPath):
                    TransformCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanTransform()
        {
            return File.Exists(SrcPath)&&!string.IsNullOrEmpty(DestPath);
        }
        /// <summary>
        /// 获取坐标转换匿名方法
        /// </summary>
        /// <returns>坐标转换匿名方法</returns>
        private Func<double, double, (double Lat, double Lon)> GetTransformFunc()
        {
            Func<double, double, (double Lat, double Lon)> ret = (lat, lon) => (lat, lon);
            switch (SrcOffsetType)
            {
                case OffsetType.None:
                    switch (DestOffsetType)
                    {
                        case OffsetType.Gcj02:
                            ret = (lat, lon) => CoordHelper.Wgs84ToGcj02(lat, lon);
                            break;
                        case OffsetType.Bd09:
                            ret = (lat, lon) => CoordHelper.Wgs84ToBd09(lat, lon);
                            break;
                    }
                    break;
                case OffsetType.Gcj02:
                    switch (DestOffsetType)
                    {
                        case OffsetType.None:
                            ret =(lat, lon) => CoordHelper.Gcj02ToWgs84(lat, lon);
                            break;
                        case OffsetType.Bd09:
                            ret =(lat, lon) => CoordHelper.Gcj02ToBd09(lat, lon);
                            break;
                    }
                    break;
                case OffsetType.Bd09:
                    switch (DestOffsetType)
                    {
                        case OffsetType.None:
                            ret =(lat, lon) => CoordHelper.Bd09ToWgs84(lat, lon);
                            break;
                        case OffsetType.Gcj02:
                            ret =(lat, lon) => CoordHelper.Bd09ToGcj02(lat, lon);
                            break;
                    }
                    break;
            }
            return ret;
        }
        private void Transform()
        {
            if (SrcOffsetType== DestOffsetType)
            {
                MessageBox.Show(Window.GetWindow(View), "不能选择相同偏移类型");
                return;
            }
            if (SrcPath==DestPath)
            {
                MessageBox.Show(Window.GetWindow(View), "目标数据目录不能与原始数据目录相同");
                return;
            }
            using var srcDataSource = Ogr.Open(SrcPath, 0);
            if (srcDataSource==null)
            {
                MessageBox.Show(Window.GetWindow(View), $"无法读取{SrcPath}");
                return;
            }
            using var driver = srcDataSource.GetDriver();
            string[] options = { "ENCODING=UTF-8" };//添加.cpg文件，以解决写入中文乱码
            using var destDataSource = driver.CopyDataSource(srcDataSource, DestPath, options);
             var layerCount = destDataSource.GetLayerCount();
            if (layerCount>0)
            {
                var layer = destDataSource.GetLayerByIndex(0);
                var featureCount = layer.GetFeatureCount(1);
                if (featureCount>0)
                {
                    var spatialReference = layer.GetSpatialRef();
                    var authorityNameAndCode = spatialReference.GetOrUpdateAuthorityNameAndCode();
                    if (authorityNameAndCode.AuthorityName=="EPSG")
                    {
                        switch (authorityNameAndCode.AuthorityCode)
                        {
                            case "4326":
                                var transformFunc = GetTransformFunc();
                                Action<double[]> transformCoordAction = (coord) =>
                                {
                                    if (transformFunc!=null&& coord!=null&& coord.Length>1)
                                    {
                                        var latLon = transformFunc(coord[1], coord[0]);
                                        coord[1]=latLon.Lat;
                                        coord[0]=latLon.Lon;
                                    }
                                };
                                for (int featureIndex = 0; featureIndex < featureCount; featureIndex++)
                                {
                                    using var feature = layer.GetFeature(featureIndex);
                                    var geometry = feature.GetGeometryRef();
                                    if (geometry==null)
                                    {
                                        Console.WriteLine($"第{featureIndex}条几何体为空");
                                        continue;
                                    }
                                    var geometryCopy = geometry.Clone();
                                    var dimension = geometryCopy.GetCoordinateDimension();
                                    if (dimension<2)
                                    {
                                        Console.WriteLine($"第{featureIndex}条纬度为{dimension}");
                                        continue;
                                    }
                                    geometryCopy.TransformCoord(transformCoordAction);
                                    feature.SetGeometry(geometryCopy);
                                    layer.SetFeature(feature);
                                    destDataSource.FlushCache();
                                }
                                MessageBox.Show(Window.GetWindow(View), "转换成功");
                                break;
                            default:
                                MessageBox.Show(Window.GetWindow(View), "坐标系必须为4326");
                                break;
                        }
                    }
                }
            }
        }
    }
}
