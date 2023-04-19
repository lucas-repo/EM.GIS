using System.Diagnostics;
using System.Reflection;
using EM.Bases;

namespace EM.GIS.MBTiles
{
    /// <summary>
    /// 元数据信息
    /// </summary>
    public class MetadataInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 格式
        /// </summary>
        public Format Format { get; set; }
        /// <summary>
        /// 最小X（经度）
        /// </summary>
        public double MinX { get; set; }
        /// <summary>
        /// 最小Y（纬度）
        /// </summary>
        public double MinY { get; set; }
        /// <summary>
        /// 最大X（经度）
        /// </summary>
        public double MaxX { get; set; }
        /// <summary>
        /// 最大Y（纬度）
        /// </summary>
        public double MaxY { get; set; }
        /// <summary>
        /// 中心X（经度）
        /// </summary>
        public double CenterX { get; set; }
        /// <summary>
        /// 中心Y（纬度）
        /// </summary>
        public double CenterY { get; set; }
        /// <summary>
        /// 最小级别
        /// </summary>
        public int MinZoom { get; set; }
        /// <summary>
        /// 最大级别
        /// </summary>
        public int MaxZoom { get; set; }
        /// <summary>
        /// 一个属性字符串，用于解释地图的数据和/或样式。
        /// </summary>
        public string? Attribution { get; set; }
        /// <summary>
        /// 描述  （字符串）
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 类型  
        /// </summary>
        public LayerType Type { get; set; }
        /// <summary>
        /// 瓦片版本  （数字）
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// 列出矢量切片中显示的图层以及显示在这些图层中要素的属性（json字符串）
        /// </summary>
        public string? json { get; set; }
        public MetadataInfo()
        { }
        public MetadataInfo(List<NameValue> nameValues)
        {
            Type type = GetType();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in nameValues)
            {
                var propertyInfo = propertyInfos.FirstOrDefault(x => x.Name.ToLower() == item.Name);
                if (propertyInfo != null)
                {
                    propertyInfo.ForceSetValue(this, item.Value);
                }
                else
                {
                    switch (item.Name)
                    {
                        case "bounds":
                            InitializeBounds(item.Value);
                            break;
                        case "center":
                            InitializeCenter(item.Value);
                            break;
                        default:
                            Debug.WriteLine($"不支持的类型 {item.Name}");
                            break;
                    }
                }
            }
        }

        private void InitializeBounds(string str)
        {
            var array = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 4)
            {
                double value;
                if (double.TryParse(array[0].Trim(), out value))
                {
                    MinX = value;
                }
                else
                {
                    throw new Exception("无法识别的值");
                }
                if (double.TryParse(array[1].Trim(), out value))
                {
                    MinY = value;
                }
                else
                {
                    throw new Exception("无法识别的值");
                }
                if (double.TryParse(array[2].Trim(), out value))
                {
                    MaxX = value;
                }
                else
                {
                    throw new Exception("无法识别的值");
                }
                if (double.TryParse(array[3].Trim(), out value))
                {
                    MaxY = value;
                }
                else
                {
                    throw new Exception("无法识别的值");
                }
            }
            else
            {
                throw new Exception("无法识别的值");
            }
        }
        private void InitializeCenter(string str)
        {
            var array = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length == 2)
            {
                double value;
                if (double.TryParse(array[0].Trim(), out value))
                {
                    CenterX = value;
                }
                else
                {
                    throw new Exception("无法识别的值");
                }
                if (double.TryParse(array[1].Trim(), out value))
                {
                    CenterY = value;
                }
                else
                {
                    throw new Exception("无法识别范围值");
                }
            }
            else
            {
                throw new Exception("无法识别范围值");
            }
        }
        /// <summary>
        /// 转为名称-值集合
        /// </summary>
        /// <returns>名称-值集合</returns>
        public List<NameValue> ToNameValues()
        {
            List<NameValue> ret = new List<NameValue>();
            Type type = GetType();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var item in propertyInfos)
            {
                var valueObj = item.GetValue(this);
                if (item.PropertyType.IsEnum)
                {
                    var value = valueObj?.ToString();
                    if (value != null)
                    {
                        NameValue nameValue = new NameValue
                        {
                            Name = item.Name.ToLower(),
                            Value = value
                        };
                        ret.Add(nameValue);
                    }
                }
                else
                {
                    switch (item.Name)
                    {
                        case nameof(MinX):
                        case nameof(MinY):
                        case nameof(MaxX):
                        case nameof(MaxY):
                            var boundNameValue = ret.FirstOrDefault(x => x.Name == "bound");
                            if (boundNameValue == null)
                            {
                                boundNameValue = new NameValue()
                                {
                                    Name = "bound",
                                    Value = $"{MinX},{MinY},{MaxX},{MaxY}"
                                };
                                ret.Add(boundNameValue);
                            }
                            break;
                        case nameof(CenterX):
                        case nameof(CenterY):
                            var centerNameValue = ret.FirstOrDefault(x => x.Name == "center");
                            if (centerNameValue == null)
                            {
                                centerNameValue = new NameValue()
                                {
                                    Name = "center",
                                    Value = $"{CenterX},{CenterY}"
                                };
                                ret.Add(centerNameValue);
                            }
                            break;
                        default:
                            if (item.PropertyType == typeof(string))
                            {
                                var value = valueObj?.ToString();
                                if (value != null)
                                {
                                    NameValue nameValue = new NameValue
                                    {
                                        Name = item.Name.ToLower(),
                                        Value = value
                                    };
                                    ret.Add(nameValue);
                                }
                            }
                            break;
                    }
                }
            }
            return ret;
        }
    }
}