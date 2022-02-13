using EM.GIS.CoordinateTransformation;
using EM.GIS.GdalExtensions;
using EM.WpfBase;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
        private string _srcDirectory;
        /// <summary>
        /// 原文件目录
        /// </summary>
        public string SrcDirectory
        {
            get { return _srcDirectory; }
            set { SetProperty(ref _srcDirectory, value); }
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

        private string _destDirectory;
        /// <summary>
        /// 目标文件目录
        /// </summary>
        public string DestDirectory
        {
            get { return _destDirectory; }
            set { SetProperty(ref _destDirectory, value); }
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
        public static string GetSelectedDirectory()
        {
            string directory = string.Empty;
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true,
                Title="请选择目录"
            };
            if (dialog.ShowDialog()== CommonFileDialogResult.Ok)
            {
                directory = dialog.FileName;
            }
            return directory;
        }
        private void SelectPath(string? pathName)
        {
            switch (pathName)
            {
                case nameof(SrcDirectory):
                    SrcDirectory=GetSelectedDirectory();
                    break;
                case nameof(DestDirectory):
                    DestDirectory=GetSelectedDirectory();
                    break;
                default:
                    return;
            }
        }

        private void CoordTransformViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SrcDirectory):
                case nameof(DestDirectory):
                    TransformCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanTransform()
        {
            return !string.IsNullOrEmpty(SrcDirectory)&&!string.IsNullOrEmpty(DestDirectory);
        }
        /// <summary>
        /// 获取坐标转换匿名方法
        /// </summary>
        /// <returns>坐标转换匿名方法</returns>
        private static Func<double, double, (double Lat, double Lon)> GetTransformFunc(OffsetType srcOffsetType, OffsetType destOffsetType)
        {
            Func<double, double, (double Lat, double Lon)> ret = (lat, lon) => (lat, lon);
            switch (srcOffsetType)
            {
                case OffsetType.None:
                    switch (destOffsetType)
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
                    switch (destOffsetType)
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
                    switch (destOffsetType)
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
        /// <summary>
        /// 校正坐标
        /// </summary>
        /// <param name="srcPath">源数据</param>
        /// <param name="destPath">目标数据</param>
        /// <param name="srcOffsetType">原坐标类型</param>
        /// <param name="destOffsetType">目标坐标类型</param>
        public static void CorrectCoords(string srcPath, string destPath, OffsetType srcOffsetType, OffsetType destOffsetType)
        {
            var transformFunc = GetTransformFunc(srcOffsetType, destOffsetType);
            CorrectCoords(srcPath, destPath, transformFunc);
        }
        /// <summary>
        /// 校正坐标
        /// </summary>
        /// <param name="srcPath">源数据</param>
        /// <param name="destPath">目标数据</param>
        /// <param name="transformFunc">校正方法</param>
        public static void CorrectCoords(string srcPath, string destPath, Func<double, double, (double Lat, double Lon)> transformFunc)
        {
            using var srcDataSource = Ogr.Open(srcPath, 0);
            if (srcDataSource==null)
            {
                Console.WriteLine($"无法读取{srcPath}");
                //MessageBox.Show(Window.GetWindow(View), $"无法读取{srcPath}");
                return;
            }
            using var driver = Ogr.GetDriverByName("ESRI Shapefile");
            using var destDataSource = driver.CopyDataSourceUTF8(srcDataSource, destPath, null);
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
                                break;
                            default:
                                Console.WriteLine("坐标系必须为4326");
                                break;
                        }
                    }
                }
            }
        }
        private void Transform()
        {
            if (SrcOffsetType== DestOffsetType)
            {
                MessageBox.Show(Window.GetWindow(View), "不能选择相同偏移类型");
                return;
            }
            if (SrcDirectory==DestDirectory)
            {
                MessageBox.Show(Window.GetWindow(View), "目标数据目录不能与原始数据目录相同");
                return;
            }
            var srcFiles = Directory.GetFiles(SrcDirectory);
            if (srcFiles.Length>0)
            {
                var transformFunc = GetTransformFunc(SrcOffsetType, DestOffsetType);
                foreach (var srcFile in srcFiles)
                {
                    var name = Path.GetFileNameWithoutExtension(srcFile);
                    var destFile = Path.Combine(DestDirectory, $"{name}.shp");
                    CorrectCoords(srcFile, destFile, transformFunc);
                }
                MessageBox.Show(Window.GetWindow(View), "转换成功");
            }
        }
    }
}
