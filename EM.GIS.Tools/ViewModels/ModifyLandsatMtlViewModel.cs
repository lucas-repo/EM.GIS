using EM.Bases;
using EM.WpfBases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace EM.GIS.Tools
{
    public class ModifyLandsatMtlViewModel : ViewModel<ModifyLandsatMtlControl>
    {
        private string _srcDirectory;
        /// <summary>
        /// 数据目录
        /// </summary>
        public string SrcDirectory
        {
            get { return _srcDirectory; }
            set { SetProperty(ref _srcDirectory, value); }
        }
        public DelegateCommand BrowseCmd { get; }
        public DelegateCommand ExcuteCmd { get; }
        public ModifyLandsatMtlViewModel(ModifyLandsatMtlControl t) : base(t)
        {
            ExcuteCmd=new DelegateCommand(Excute, CanExcute);
            BrowseCmd=new DelegateCommand(Browse);
            PropertyChanged+=ModifyLandsatMtlViewModel_PropertyChanged;
        }

        private void Browse()
        {
            SrcDirectory=WindowExtensions.GetSelectedFolder(window: Window.GetWindow(View));
        }
        private void ModifyLandsatMtlViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SrcDirectory):
                    ExcuteCmd.RaiseCanExecuteChanged();
                    break;
            }
        }

        private bool CanExcute()
        {
            return Directory.Exists(SrcDirectory);
        }

        private void Excute()
        {
            var directories = new List<string>() { SrcDirectory};
            directories.AddRange(Directory.GetDirectories(SrcDirectory));
            foreach (var item in directories)
            {
                var name = Path.GetFileNameWithoutExtension(item);
                var mtlCopyPath = Path.Combine(item, $"{name}_MTL - 副本.txt");
                if (File.Exists(mtlCopyPath))
                {
                    continue;
                }
                var mtlPath = Path.Combine(item, $"{name}_MTL.txt");
                if (!File.Exists(mtlPath))
                {
                    continue;
                }
                File.Copy(mtlPath, mtlCopyPath,true);
                using StreamReader sr = new StreamReader(mtlPath);
                StringBuilder sb = new StringBuilder();
                var firstLineStr = sr.ReadLine();
                if (firstLineStr!=null)
                {
                    sb.AppendLine(firstLineStr.Replace("LANDSAT_METADATA_FILE", "L1_METADATA_FILE"));
                }
                var str = sr.ReadLine();
                bool containsLevel1 = false;
                while (str!=null)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (containsLevel1)
                        {
                            containsLevel1=!str.Contains("END_GROUP = LEVEL1");
                        }
                        else
                        {
                            containsLevel1=str.Contains("GROUP = LEVEL1");
                            if (!containsLevel1)
                            {
                                sb.AppendLine(str);
                            }
                            else
                            { 
                            }
                        }
                    }
                    str= sr.ReadLine();
                }
                sr.Close();
                File.WriteAllText(mtlPath, sb.ToString());
            }
            MessageBox.Show("OK");
        }
    }
}
