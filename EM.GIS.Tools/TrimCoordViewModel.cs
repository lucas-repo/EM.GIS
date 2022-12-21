using EM.Bases;
using EM.WpfBases;
using Microsoft.Win32;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Shapes;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 整理坐标视图模型
    /// </summary>
    /// <typeparam name="TrimCoordControl"></typeparam>
    public class TrimCoordViewModel : ViewModel<TrimCoordControl>
    {
        private string _path;
        /// <summary>
        /// 路径
        /// </summary>
        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }
        /// <summary>
        /// 列集合
        /// </summary>
        public ObservableCollection<KeyValueClass<int,string>> Columns { get; } = new ObservableCollection<KeyValueClass<int, string>>();

        private KeyValueClass<int, string> _xCol;
        /// <summary>
        /// x列
        /// </summary>
        public KeyValueClass<int, string> XCol
        {
            get { return _xCol; }
            set { SetProperty(ref _xCol, value); }
        }
        private KeyValueClass<int, string> _yCol;
        /// <summary>
        /// y列
        /// </summary>
        public KeyValueClass<int, string> YCol
        {
            get { return _yCol; }
            set { SetProperty(ref _yCol, value); }
        }
        private KeyValueClass<int, string> _newXCol;
        /// <summary>
        /// 新x列
        /// </summary>
        public KeyValueClass<int, string> NewXCol
        {
            get { return _newXCol; }
            set { SetProperty(ref _newXCol, value); }
        }
        private KeyValueClass<int, string> _newYCol;
        /// <summary>
        /// 新y列
        /// </summary>
        public KeyValueClass<int, string> NewYCol
        {
            get { return _newYCol; }
            set { SetProperty(ref _newYCol, value); }
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        public DelegateCommand ExcuteCmd { get; }
        private XSSFWorkbook _sheets;

        public XSSFWorkbook Sheets
        {
            get { return _sheets; }
            set
            {
                if (_sheets != null)
                {
                    _sheets.Dispose();
                }
                _sheets = value;
            }
        }

        /// <summary>
        /// 选择目录命令
        /// </summary>
        public DelegateCommand SelectPathCmd { get; }
        public TrimCoordViewModel(TrimCoordControl t) : base(t)
        {
            ExcuteCmd = new DelegateCommand(Excute);
            PropertyChanged += TrimCoordViewModel_PropertyChanged;
            SelectPathCmd = new DelegateCommand(SelectPath);
        }

        private void SelectPath()
        {
            OpenFileDialog dg = new OpenFileDialog()
            {
                Filter = "*.xlsx|*.xlsx"
            };
            if (dg.ShowDialog() == true)
            {
                Path = dg.FileName;
            }
        }
        private void TrimCoordViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Path):
                    Columns.Clear();
                    Sheets = new XSSFWorkbook(Path);
                    if (Sheets.Count > 0)
                    {
                        var sheet = Sheets[0];
                        if (sheet.LastRowNum >= 0)
                        {
                            var firstRow = sheet.GetRow(0);
                            for (int i = 0; i < firstRow.LastCellNum; i++)
                            {
                                var cell = firstRow.GetCell(i);
                                if (cell != null)
                                {
                                    var col = cell.ToString();
                                    if (!string.IsNullOrEmpty(col))
                                    {
                                        Columns.Add(new KeyValueClass<int, string>(i, col));
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void Excute()
        {
            if (!File.Exists(Path))
            {
                MessageBox.Show(Window.GetWindow(View), "请选择文件路径");
                return;
            }
            if (XCol==null || YCol==null || NewXCol==null || NewYCol==null)
            {
                MessageBox.Show(Window.GetWindow(View), "请选择列");
                return;
            }
            var sheet = Sheets[0];
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);
                var xStr= row.GetCell(XCol.Key).ToString();
                var destX = GetDoubleValue(xStr);
                if (destX.HasValue)
                {
                    row.GetCell(NewXCol.Key).SetCellValue(destX.Value);
                }
                var yStr = row.GetCell(YCol.Key).ToString();
                var destY = GetDoubleValue(yStr);
                if (destY.HasValue)
                {
                    row.GetCell(NewYCol.Key).SetCellValue(destY.Value);
                }
            }
            string directory = System.IO.Path.GetDirectoryName(Path);
            string name = System.IO.Path.GetFileNameWithoutExtension(Path);
            string extension = System.IO.Path.GetExtension(Path);
            string newPath = System.IO.Path.Combine(directory,$"{name}1{extension}");
            using (FileStream fs = File.OpenWrite(newPath))
            {
                Sheets.Write(fs);
            }
            MessageBox.Show(Window.GetWindow(View), "成功");
        }
        private double? GetDoubleValue(string? str)
        {
            double? ret = null;
            if (string.IsNullOrEmpty(str))
            {
                return ret;
            }
            if (double.TryParse(str, out double val))
            {
                ret = val;
            }
            else
            {
                char[] sparator = { '°', '′','\'' , '’' };
                var strs= str.Split(sparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries); 
                string pattern = "[0-9]+([.]{1}[0-9]+){0,1}";
                Regex regex = new Regex(pattern);
                ret = 0;
                for (int i = 0; i < strs.Length; i++)
                {
                    var match = regex.Match(strs[i]);
                    if (match.Success)
                    {
                        string matchedStr = match.Value;
                        if (double.TryParse(matchedStr, out double val1))
                        {
                            switch (i)
                            {
                                case 0:
                                    ret += val1;
                                    break;
                                case 1:
                                    ret += val1 / 60;
                                    break;
                                case 2:
                                    ret += val1 / 3600;
                                    break;
                            }
                        }
                        else
                        { }
                    }
                    else
                    {
                        return ret;
                    }
                }
            }
            return ret;
        }
    }
}