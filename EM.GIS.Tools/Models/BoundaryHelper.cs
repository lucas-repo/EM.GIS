using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 行政界线辅助类
    /// </summary>
    public static class BoundaryHelper
    {
        /// <summary>
        /// 获取后缀
        /// </summary>
        /// <param name="includeChildren">包含子区域</param>
        /// <returns>后缀</returns>
        public static string GetExtensions(bool includeChildren = true)
        {
            return includeChildren ? "_full.json" : ".json";
        }
        /// <summary>
        /// 获取阿里云下载行政界线的地址（火星坐标系）
        /// </summary>
        /// <param name="adcode">行政区划代码</param>
        /// <param name="includeChildren">包含子区域</param>
        /// <param name="version">版本</param>
        /// <param name="domain">域名</param>
        /// <returns>地址</returns>
        public static string GetBoundaryUrl(int adcode, bool includeChildren =true,string version= "areas_v3",string domain = "https://geo.datav.aliyun.com")
        {
            return $"{domain}/{version}/bound/{adcode}{GetExtensions(includeChildren)}";
        }
        public static string GetJsonPath(string directory,string country, string? province=null, string? city = null, string? district = null,bool includeChildren = true)
        {
            string name = "未知";
            if (district != null)
            {
                name = $@"{country}\{province}\{city}\{district}";
            }
            else if (city != null)
            {
                name = $@"{country}\{province}\{city}";
            }
            else if (province != null)
            {
                name = $@"{country}\{province}";
            }
            else if (country != null)
            {
                name = $"{country}";
            }
            else
            {
                throw new Exception("参数不能为空");
            }
            var jsonPath = Path.Combine(directory, $"{name}{GetExtensions(includeChildren)}");
            return jsonPath;
        }
        /// <summary>
        /// 获取行政区划集合
        /// </summary>
        /// <returns>行政区划集合</returns>
        public static List<CityInfo> GetCityInfos()
        {
            var cityInfos = new List<CityInfo>();
            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument doc = webClient.Load("https://www.mca.gov.cn/article/sj/xzqh/2020/20201201.html");
            //查找节点
            var root=  doc.DocumentNode.ChildNodes.FirstOrDefault();
            if (root != null)
            {
                var array=  root.InnerText.Split(new string[] { " ", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                int adcode = 0;
                CityInfo? provinceInfo = null;//省
                CityInfo? cityInfo = null;//市
                foreach (var item in array) 
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        continue;
                    }
                    if (adcode == 0)
                    {
                        int.TryParse(item, out adcode);
                    }
                    else
                    {
                        CityInfo itemInfo = new CityInfo(adcode, item);
                        if (adcode % 10000 == 0)//省级
                        {
                            provinceInfo = itemInfo;
                            cityInfo = null;
                            cityInfos.Add(itemInfo);
                        }
                        else if (adcode % 100 == 0)//市
                        {
                            cityInfo = itemInfo;
                            if (provinceInfo != null)
                            {
                                provinceInfo.Children.Add(itemInfo);
                            }
                            else
                            { }
                        }
                        else
                        {
                            if (cityInfo != null)
                            {
                                cityInfo.Children.Add(itemInfo);
                            }
                            else if (provinceInfo != null)
                            {
                                provinceInfo.Children.Add(itemInfo);
                            }
                            else
                            { }
                        }

                        adcode = 0;
                    }
                }
            }
            return cityInfos;
        }
    }
}
