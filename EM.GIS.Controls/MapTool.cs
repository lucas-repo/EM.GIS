using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 地图工具
    /// </summary>
    public abstract class MapTool : IMapTool
    {
        #region  Constructors

        public MapTool()
        {

        }

        public MapTool(IMap map)
        {
            Map = map;
        }

        #endregion

        #region Events

        public event EventHandler Activated;
        public event EventHandler Deactivated;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<GeoMouseArgs> MouseDoubleClick;
        public event EventHandler<GeoMouseArgs> MouseDown;
        public event EventHandler<GeoMouseArgs> MouseMove;
        public event EventHandler<GeoMouseArgs> MouseUp;
        public event EventHandler<GeoMouseArgs> MouseWheel;
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

        public virtual void DoKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        public virtual void DoKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        public virtual void DoMouseDoubleClick(GeoMouseArgs e)
        {
            MouseDoubleClick?.Invoke(this, e);
        }

        public virtual void DoMouseDown(GeoMouseArgs e)
        {
            MouseDown?.Invoke(this, e);
        }

        public virtual void DoMouseMove(GeoMouseArgs e)
        {
            MouseMove?.Invoke(this, e);
        }

        public virtual void DoMouseUp(GeoMouseArgs e)
        {
            MouseUp?.Invoke(this, e);
        }

        public virtual void DoMouseWheel(GeoMouseArgs e)
        {
            MouseWheel?.Invoke(this, e);
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
