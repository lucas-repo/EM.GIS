using EM.GIS.Symbology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 右键命令
    /// </summary>
    public class ContextCommand : Command,IContextCommand
    {
        private string name;
        /// <summary>
        /// 命令名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private string header;
        /// <summary>
        /// 显示的文字
        /// </summary>
        public string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }
        private string toolTip;
        /// <summary>
        /// 提示
        /// </summary>
        public string ToolTip
        {
            get { return toolTip; }
            set { SetProperty(ref toolTip, value); }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource? Image
        {
            get 
            {
                if ((this as IContextCommand).Image is ImageSource imageSource)
                {
                    return imageSource;
                }
                else
                {
                    return null;
                }
            }
            set 
            {
                if ((this as IContextCommand).Image != value)
                {
                    (this as IContextCommand).Image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource? LargeImage
        {
            get
            {
                if ((this as IContextCommand).LargeImage is ImageSource imageSource)
                {
                    return imageSource;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((this as IContextCommand).LargeImage != value)
                {
                    (this as IContextCommand).LargeImage = value;
                    OnPropertyChanged(nameof(LargeImage));
                }
            }
        }
        object? IContextCommand.Image { get; set; }
        object? IContextCommand.LargeImage { get; set; }
        protected ContextCommand()
        { }
        public ContextCommand(Action<object?> excute) : base(excute)
        {
        }
        public ContextCommand(Action<object?> excute, Func<object?, bool> canExecute) : base(excute, canExecute)
        {
        }
    }
}
