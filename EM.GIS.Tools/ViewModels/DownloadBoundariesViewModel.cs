using EM.WpfBase;
using OSGeo.OGR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EM.GIS.GdalExtensions;
using Microsoft.Win32;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 行政界线下载视图模型
    /// </summary>
    public class DownloadBoundariesViewModel : ViewModel<DownloadBoundariesControl>
    {
        /// <summary>
        /// 域名
        /// </summary>
        private const string Domain = "https://geo.datav.aliyun.com";

        /// <summary>
        /// 版本
        /// </summary>
        public ObservableCollection<string> Versions { get; }
        private string _version;
        /// <summary>
        /// 版本
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { SetProperty(ref _version, value); }
        }
        /// <summary>
        /// 市
        /// </summary>
        public ObservableCollection<TreeItem<int>> Cities { get; }
        private TreeItem<int> _city;
        /// <summary>
        /// 市
        /// </summary>
        public TreeItem<int> City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }
        private bool _includeChildren = true;
        /// <summary>
        /// 包含子区域
        /// </summary>
        public bool IncludeChildren
        {
            get { return _includeChildren; }
            set { SetProperty(ref _includeChildren, value); }
        }
        private string _url;
        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { SetProperty(ref _url, value); }
        }

        private bool _isCorrected = true;
        /// <summary>
        /// 校正
        /// </summary>
        public bool IsCorrected
        {
            get { return _isCorrected; }
            set { SetProperty(ref _isCorrected, value); }
        }

        private string _shpPath;
        /// <summary>
        /// 保存目录
        /// </summary>
        public string ShpPath
        {
            get { return _shpPath; }
            set { SetProperty(ref _shpPath, value); }
        }
        public DelegateCommand BrowseCmd { get; }
        /// <summary>
        /// 下载命令
        /// </summary>
        public DelegateCommand DownloadCmd { get; }
        public DownloadBoundariesViewModel(DownloadBoundariesControl t) : base(t)
        {
            _url="https://geo.datav.aliyun.com";
            Versions=new ObservableCollection<string> { "areas_v2", "areas_v3" };
            _version=Versions.Last();
            Cities=new ObservableCollection<TreeItem<int>>()
            {
                new TreeItem<int>(100000,"中华人民共和国")
            };
            _city=Cities.First();
            _shpPath=Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", "行政界线/中国.shp");
            PropertyChanged+=DownloadBoundariesViewModels_PropertyChanged;
            BrowseCmd=new DelegateCommand(Browse);
            DownloadCmd=new DelegateCommand(Download);

        }

        private void Browse()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title="选择要保存的位置",
                Filter="*.shp|*.shp"
            };
            if (saveFileDialog.ShowDialog()==true)
            {
                ShpPath=saveFileDialog.FileName;
            }
        }

        private HttpClient? _httpClient;
        /// <summary>
        /// http客户端
        /// </summary>
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient==null)
                {
                    _httpClient=new HttpClient();
                }
                return _httpClient;
            }
        }

        private void Download()
        {
            //下载
            var stream = HttpClient.GetStreamAsync(ShpPath).Result;
            if (File.Exists(ShpPath))
            {
                File.Delete(ShpPath);
            }
            var name = Path.GetFileNameWithoutExtension(ShpPath);
            var jsonPath = ShpPath.Replace($"{name}.shp", $"{name}.json");
            using var fs=File.OpenWrite(jsonPath);
            stream.CopyTo(fs);
            //校正
            if (IsCorrected)
            {
                CoordTransformViewModel.CorrectCoords(jsonPath, ShpPath, OffsetType.Gcj02, OffsetType.None);
            }
            else
            {
                using var srcDs = Ogr.Open(jsonPath, 0);
                using var driver = Ogr.GetDriverByName("ESRI Shapefile");
                using var destDs = driver.CopyDataSourceUTF8(srcDs, ShpPath, null);
            }
            File.Delete(jsonPath);
            MessageBox.Show(Window.GetWindow(View), "OK");
        }

        private void DownloadBoundariesViewModels_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Version):
                case nameof(City):
                    string extension = IncludeChildren ? $"_full.json" : ".json";
                    Url=$"{Domain}/{Version}/bound/{City?.Value}{extension}";
                    break;
            }
        }
    }
}
