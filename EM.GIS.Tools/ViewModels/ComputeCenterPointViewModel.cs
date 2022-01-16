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
        /// <summary>
        /// 执行命令
        /// </summary>
        public DelegateCommand ExcuteCmd { get; }
        /// <summary>
        /// 选择目录命令
        /// </summary>
        public DelegateCommand<string> SelectPathCmd { get; }
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
        
        private void Excute()
        {
            using DataSource srcDataSource = Ogr.Open(SrcPath, 0);
            if (srcDataSource==null)
            {
                MessageBox.Show(Window.GetWindow(View), $"无法打开{SrcPath}");
                return;
            }
            using var srcLayer = srcDataSource.GetLayerByIndex(0);
            if (srcLayer==null)
            {
                MessageBox.Show(Window.GetWindow(View), $"{SrcPath}没有图层数据");
                return;
            }
            var srcFeatureCount = srcLayer.GetFeatureCount(1);
            if (srcFeatureCount==0)
            {
                MessageBox.Show(Window.GetWindow(View), $"{SrcPath}没有数据");
                return;
            }
            using DataSource maskDataSource = Ogr.Open(MaskPath, 0);
            if (maskDataSource==null)
            {
                MessageBox.Show(Window.GetWindow(View), $"无法打开{MaskPath}");
                return;
            }
            using var maskLayer = maskDataSource.GetLayerByIndex(0);
            if (maskLayer==null)
            {
                MessageBox.Show(Window.GetWindow(View), $"{MaskPath}没有图层数据");
                return;
            }
            var maskFeatureCount = maskLayer.GetFeatureCount(1);
            if (maskFeatureCount==0)
            {
                MessageBox.Show(Window.GetWindow(View), $"{MaskPath}没有数据");
                return;
            }
            using Driver driver = srcDataSource.GetDriver();
            using var destDataSource = driver.CreateDataSource(DestPath, null);
            var destName = Path.GetFileNameWithoutExtension(DestPath);
            using var destLayer = destDataSource.CreateLayer(destName, srcLayer);//创建图层
            //根据叠加图层随机取相交的原始要素集合
            var centroidAndAroundFeatures = new Dictionary<Geometry, List<Feature>>();//中心点及周围的要素
            for (long i = 0; i < maskFeatureCount; i++)
            {
                using var maskFeature = maskLayer.GetFeature(i);
                var maskGeometry = maskFeature.GetGeometryRef();//返回的几何图形仍归容器所有，不应修改
                srcLayer.SetSpatialFilter(maskGeometry);
                using var srcFeature = srcLayer.GetNextFeature();

                if (srcFeature!=null)
                {
                    centroidAndAroundFeatures.Add(srcFeature.GetGeometryRef().Centroid(), new List<Feature>());
                }
            }
            //开始迭代
            if (centroidAndAroundFeatures.Count>0)
            {
                for (int i = 0; i < IterationCount; i++)
                {
                    var newSrcFeatures = new List<Feature>();
                    //计算离中心点较近的要素
                    for (int srcFeatureIndex = 0; srcFeatureIndex < srcFeatureCount; srcFeatureIndex++)
                    {
                        using var tmpFeature = srcLayer.GetFeature(srcFeatureIndex);
                        double minDistance = double.MaxValue;
                        Geometry? minDistanceGeometry = null;
                        foreach (var centerAndAroundFeaturePair in centroidAndAroundFeatures)
                        {
                            var centroidGeometry = centerAndAroundFeaturePair.Key;
                            double distance = centroidGeometry.Distance(tmpFeature.GetGeometryRef());
                            if (distance<minDistance)
                            {
                                minDistance=distance;
                                minDistanceGeometry =centerAndAroundFeaturePair.Key;
                            }
                        }
                        if (minDistanceGeometry!=null)
                        {
                            centroidAndAroundFeatures[minDistanceGeometry].Add(tmpFeature);
                        }
                    }
                    var newCentroidAndAroundFeatures = new Dictionary<Geometry, List<Feature>>();//中心点及周围的要素
                    foreach (var centerAndAroundFeaturePair in centroidAndAroundFeatures)
                    {
                        var newCentroid = centerAndAroundFeaturePair.Value.Select(x => x.GetGeometryRef().Centroid()).GetCentroid();
                        var oldCentrolid = centerAndAroundFeaturePair.Key;
                        newCentroidAndAroundFeatures[newCentroid]=centerAndAroundFeaturePair.Value;
                        oldCentrolid.Dispose();
                    }
                    centroidAndAroundFeatures=newCentroidAndAroundFeatures;
                }
            }
        }
    }
}
