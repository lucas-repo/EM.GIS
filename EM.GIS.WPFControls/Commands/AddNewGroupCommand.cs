using EM.GIS.Resources;
using EM.GIS.Symbology;
using System.Windows;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 添加新图层组命令
    /// </summary>
    public class AddNewGroupCommand:ContextCommand
    {
        public AddNewGroupCommand(IGroup group)
        {
            Name = nameof(AddNewGroupCommand);
            Header = "添加分组";
            ToolTip = "添加分组至当前分组下";
            Image = ResourcesHelper.GetBitmapImage("Global16.png");
            LargeImage = ResourcesHelper.GetBitmapImage("Global32.png");
            ExcuteAction = (obj =>
            {
                InputWindow window = new InputWindow(value: "图层组")
                {
                    Owner= Application.Current.MainWindow
                };
                if (window.ShowDialog() == true)
                {
                    var childGroup= group.Children.AddGroup(window.Value);
                    if (childGroup != null)
                    {
                        childGroup.Frame=group.Frame;
                    }
                }
            });
        }
    }
}
