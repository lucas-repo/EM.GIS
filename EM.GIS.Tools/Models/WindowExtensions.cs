using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;

namespace EM.GIS.Tools
{
    public static class WindowExtensions
    {
        /// <summary>
        /// 获取选择的文件夹
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="window">窗体</param>
        /// <returns>文件夹</returns>
        public static string GetSelectedFolder(string title = "请选择目录", Window window = null)
        {
            string folder = string.Empty;
            CommonOpenFileDialog dg = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Title =title
            };
            if (dg.ShowDialog(window)== CommonFileDialogResult.Ok)
            {
                folder=dg.FileName;
            }
            return folder;
        }
    }
}
