using EM.IOC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 工具
    /// </summary>
    public abstract class Tool : ITool
    {
        #region Events

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<MapEventArgs> Drawn;


        #endregion

        #region Properties

        public bool IsActivated { get; protected set; }

        public bool BusySet { get; set; }
        private string _name;

        public string Name
        {
            get => _name;
            set => _name =value;
        }

        public MapToolMode MapToolMode { get; set; }
        public Image Image { get; set; }
        public Stream Cursor { get; set; }
       
        #endregion

        #region Methods

        public virtual void Activate()
        {
            IsActivated = true;
            Activated?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Deactivate()
        {
            IsActivated = false;
            Deactivated?.Invoke(this, EventArgs.Empty);
        }

        public virtual void DoDraw(MapEventArgs e)
        {
            Drawn?.Invoke(this, e);
        }

        public string GetAvailableName(IEnumerable<ITool> tools,  string baseName)
        {
            string newName = baseName;
            int i = 1;
            if (tools == null)
            {
                return newName;
            }
                string name = newName;
                bool found = tools.Any(function => function.Name == name);
                while (found)
                {
                    newName = baseName + i;
                    i++;
                    string name1 = newName;
                    found = tools.Any(function => function.Name == name1);
                }

            return newName;
        }

        #endregion
    }
}
