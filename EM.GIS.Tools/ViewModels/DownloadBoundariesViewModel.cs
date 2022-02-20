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
        /// 国家
        /// </summary>
        public ObservableCollection<CityInfo> Countries { get; }
        private CityInfo? _country;
        /// <summary>
        /// 国家
        /// </summary>
        public CityInfo? Country
        {
            get { return _country; }
            set { SetProperty(ref _country, value); }
        }
        /// <summary>
        /// 省
        /// </summary>
        public ObservableCollection<CityInfo> Provinces { get; }
        private CityInfo? _province;
        /// <summary>
        /// 省
        /// </summary>
        public CityInfo? Province
        {
            get { return _province; }
            set { SetProperty(ref _province, value); }
        }
        /// <summary>
        /// 市
        /// </summary>
        public ObservableCollection<CityInfo> Cities { get; }
        private CityInfo? _city;
        /// <summary>
        /// 市
        /// </summary>
        public CityInfo? City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }
        /// <summary>
        /// 区
        /// </summary>
        public ObservableCollection<CityInfo> Districts { get; }
        private CityInfo _district;
        /// <summary>
        /// 区
        /// </summary>
        public CityInfo District
        {
            get { return _district; }
            set { SetProperty(ref _district, value); }
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
        private bool _IncludeChildrenVisible=true;
        /// <summary>
        /// 包含子区域是否可见
        /// </summary>
        public bool IncludeChildrenVisible
        {
            get { return _IncludeChildrenVisible; }
            set { SetProperty(ref _IncludeChildrenVisible, value); }
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
            Versions=new ObservableCollection<string> { "areas_v2", "areas_v3" };
            _version=Versions.Last();
            Countries=new ObservableCollection<CityInfo>()
            {
                new CityInfo(100000,"中华人民共和国")
            };
            Provinces=new ObservableCollection<CityInfo>();
            Cities=new ObservableCollection<CityInfo>();
            Districts=new ObservableCollection<CityInfo>();
            _shpPath=Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", @"行政界线\中国.shp");
            BrowseCmd=new DelegateCommand(Browse);
            DownloadCmd=new DelegateCommand(Download);

            PropertyChanged+=DownloadBoundariesViewModels_PropertyChanged;
            Country=Countries.First();
        }

        private void Browse()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title="选择要保存的位置",
                Filter="*.shp|*.shp",
                FileName=ShpPath
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
        private string GetJsonPath()
        {
            return GetJsonPath(Country?.Text, Province?.Text, City?.Text, District?.Text);
        }
        private string GetJsonPath(string? country, string? province, string? city, string? district )
        {
            string name = "未知";
            if (district!=null)
            {
                name=$@"{country}\{province}\{city}\{district}";
            }
            else if (city!=null)
            {
                name=$@"{country}\{province}\{city}";
            }
            else if (province!=null)
            {
                name=$@"{country}\{province}";
            }
            else if (country!=null)
            {
                name=$"{country}";
            }
            string extension = IncludeChildren ? $"_full.json" : ".json";
            var jsonPath = Path.Combine($"{AppDomain.CurrentDomain.BaseDirectory}", $@"行政界线\{name}{extension}");
            return jsonPath;
        }
        private string GetJsonPath(string parentJsonPath, string name)
        {
            string[] array = parentJsonPath.Split('_');
            string directory = Path.Combine(Path.GetDirectoryName(parentJsonPath), array[0]);
            string extension = IncludeChildren ? $"_full.json" : ".json";
            var jsonPath = Path.Combine(directory, $@"{name}{extension}");
            return jsonPath;
        }
        private void DownloadJson(string url, string jsonPath)
        {
            if (string.IsNullOrEmpty(url))
            {
                Console.WriteLine("地址为空");
                return;
            }
            if (!File.Exists(jsonPath))
            {
                var stream = HttpClient.GetStreamAsync(url).Result;
                var directory = Path.GetDirectoryName(jsonPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                using var fs = File.OpenWrite(jsonPath);
                stream.CopyTo(fs);
            }
        }
        private void Download()
        {
            string jsonPath = GetJsonPath();
            var tempJsonPath = $"{Path.GetTempFileName()}.json";
            File.Copy(jsonPath, tempJsonPath);
            using var ds = Ogr.Open(tempJsonPath, 1);
            if (ds!=null)
            {
                var layer = ds.GetLayerByIndex(0);
                var url = GetUrl(Country?.Value, Province?.Value, City?.Value, District?.Value);
                DownLoad(layer, jsonPath, url, false);
                ds.FlushCache();
            }
            //校正
            if (IsCorrected)
            {
                CoordTransformViewModel.CorrectCoords(tempJsonPath, ShpPath, OffsetType.Gcj02, OffsetType.None);
            }
            else
            {
                using var driver = Ogr.GetDriverByName("ESRI Shapefile");
                using var destDs = driver.CopyDataSourceUTF8(ds, ShpPath, null);
            }
            MessageBox.Show(Window.GetWindow(View), "OK");
        }
        private void DownLoad(Layer? parentLayer, string jsonPath, string url, bool append)
        {
            DownloadJson(url, jsonPath);
            using var ds = Ogr.Open(jsonPath, 0);
            var layer = ds?.GetLayerByIndex(0);
            if (layer!=null)
            {
                var featureCount = layer.GetFeatureCount(0);
                if (featureCount>0)
                {
                    if (append)
                    {
                        parentLayer.CopyLayer(layer);//复制图层数据
                    }

                    var featureDefn = layer.GetLayerDefn();
                    var nameFieldIndex = featureDefn.GetFieldIndex("name");//名称字段索引
                    if (nameFieldIndex!=-1)
                    {
                        for (long i = 0; i < featureCount; i++)
                        {
                            using var feature = layer.GetFeature(i);
                            var childrenNu = feature.GetFieldAsInteger("childrenNum");
                            if (childrenNu==0)
                            {
                                continue;
                            }
                            var name = feature.GetFieldAsStringUTF8(nameFieldIndex);
                            var code = feature.GetFieldAsInteger("adcode");
                            var childJsonPath = GetJsonPath(jsonPath, name);
                            var childUrl = GetUrl(code);
                            DownLoad(parentLayer, childJsonPath, childUrl, true);
                        }
                    }
                }
            }
        }
        private List<CityInfo> GetChildren(string jsonPath)
        {
            List<CityInfo> children = new List<CityInfo>();
            using var srcDs = Ogr.Open(jsonPath, 0);
            if (srcDs==null)
            {
                return children;
            }
            var layer = srcDs.GetLayerByIndex(0);
            if (layer==null)
            {
                return children;
            }
            var count = layer.GetFeatureCount(0);
            var featureDefn = layer.GetLayerDefn();
            var nameFieldIndex = featureDefn.GetFieldIndex("name");//名称字段索引
            if (nameFieldIndex!=-1)
            {
                for (long i = 0; i < count; i++)
                {
                    using var feature = layer.GetFeature(i);
                    var name = feature.GetFieldAsStringUTF8(nameFieldIndex);
                    var code = feature.GetFieldAsInteger("adcode");
                    var childrenNum = feature.GetFieldAsInteger("childrenNum");
                    children.Add(new CityInfo(code, name) { ChildrenNum=childrenNum });
                }
            }
            return children;
        }
        private bool _ignore;
        private void AddTreeItems(string jsonPath, ObservableCollection<CityInfo> treeItems)
        {
            if (!_ignore)
            {
                _ignore=true;
                treeItems.Clear();
                List<CityInfo> children = GetChildren(jsonPath);
                foreach (var item in children)
                {
                    treeItems.Add(item);
                }
                _ignore=false;
            }
        }
        private void DownloadBoundariesViewModels_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            string jsonPath, url;
            switch (e.PropertyName)
            {
                case nameof(Country):
                    if (Country!=null)
                    {
                        url=GetUrl(Country.Value, null, null, null);
                        jsonPath = GetJsonPath(Country.Text, null, null, null);
                        DownloadJson(url, jsonPath);
                        AddTreeItems(jsonPath,Provinces);
                        Country.ChildrenNum=Provinces.Count;
                        IncludeChildrenVisible=Country.ChildrenNum>0;
                    }
                    break;
                case nameof(Province):
                    if (Province!=null)
                    {
                        IncludeChildrenVisible=Province.ChildrenNum>0;
                        url=   GetUrl(Country?.Value, Province.Value, null, null);
                        jsonPath = GetJsonPath(Country?.Text, Province.Text, null, null);
                        DownloadJson(url, jsonPath);
                        AddTreeItems(jsonPath, Cities);
                    }
                    break;
                case nameof(City):
                    if (City!=null)
                    {
                        IncludeChildrenVisible=City.ChildrenNum>0;
                        url=   GetUrl(Country?.Value, Province?.Value, City.Value, null);
                        jsonPath = GetJsonPath(Country?.Text, Province?.Text, City.Text, null);
                        DownloadJson(url, jsonPath);
                        AddTreeItems(jsonPath, Districts);
                    }
                    break;
                case nameof(District):
                    if (District!=null)
                    {
                        IncludeChildrenVisible=District.ChildrenNum>0;
                        url=   GetUrl(Country?.Value, Province?.Value, City?.Value, District.Value);
                        jsonPath = GetJsonPath(Country?.Text, Province?.Text, City?.Text, District.Text);
                        DownloadJson(url, jsonPath);
                    }
                    break;
            }
        }
        private string GetUrl(int adcode)
        {
            bool includeChildren = IncludeChildren&&IncludeChildrenVisible;
            string extension = includeChildren ? $"_full.json" : ".json";
            return $"{Domain}/{Version}/bound/{adcode}{extension}";
        }
        private string GetUrl(int? countryCode, int? provinceCode, int? cityCode, int? districtCode)
        {
            string url = string.Empty;
            int adcode;
            if (districtCode!=null)
            {
                adcode=districtCode.Value;
            }
            else if (cityCode!=null)
            {
                adcode=cityCode.Value;
            }
            else if (provinceCode!=null)
            {
                adcode=provinceCode.Value;
            }
            else if (countryCode!=null)
            {
                adcode=countryCode.Value;
            }
            else
            {
                return url;
            }
            url=GetUrl(adcode);
            return url;
        }
    }
}
