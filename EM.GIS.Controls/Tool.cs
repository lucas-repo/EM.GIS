using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 工具
    /// </summary>
    public abstract class Tool : ITool
    {
        #region  Constructors

        public Tool()
        {

        }

        public Tool(IMap map)
        {
            Map = map;
        }

        #endregion

        #region Events

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<MapDrawArgs> Drawn;


        #endregion

        #region Properties

        public bool IsActivated { get; protected set; }

        public IMap Map { get; set; }
        public bool BusySet { get; set; }
        private string _name;

        public string Name
        {
            get => _name;
            set => _name = GetAvailableName(value);
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

        public virtual void DoDraw(MapDrawArgs e)
        {
            Drawn?.Invoke(this, e);
        }

        public string GetAvailableName(string baseName)
        {
            string newName = baseName;
            int i = 1;
            if (Map?.MapTools != null)
            {
                string name = newName;
                bool found = Map.MapTools.Any(function => function.Name == name);
                while (found)
                {
                    newName = baseName + i;
                    i++;
                    string name1 = newName;
                    found = Map.MapTools.Any(function => function.Name == name1);
                }
            }

            return newName;
        }

        #endregion
    }
}
