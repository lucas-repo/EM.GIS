using EM.GIS.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace EM.GIS.WPFControls
{
    public class WpfPlugin : Plugin
    {
        public new IWpfAppManager App
        {
            get => base.App as IWpfAppManager;
            set => base.App = value;
        }
    }
}
