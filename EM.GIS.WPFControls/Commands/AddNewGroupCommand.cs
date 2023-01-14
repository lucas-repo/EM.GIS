using EM.GIS.Resources;
using EM.GIS.Symbology;

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
                InputWindow window = new InputWindow(value: "图层组");
                if (window.ShowDialog() == true)
                {
                    var newGroup = new Group()
                    {
                        Text = window.Value
                    };
                    group.Children.Add(newGroup);
                }
            });
        }
    }
}
