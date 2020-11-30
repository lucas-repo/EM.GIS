using System;
using System.Runtime.InteropServices;

namespace EM.GIS.Controls
{
    [ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {
        private readonly Keys keyData;

        private bool handled;

        private bool suppressKeyPress;

        public virtual bool Alt => (keyData & Keys.Alt) == Keys.Alt;

        public bool Control => (keyData & Keys.Control) == Keys.Control;

        public bool Handled
        {
            get
            {
                return handled;
            }
            set
            {
                handled = value;
            }
        }

        public Keys KeyCode
        {
            get
            {
                Keys keys = keyData & Keys.KeyCode;
                if (!Enum.IsDefined(typeof(Keys), (int)keys))
                {
                    return Keys.None;
                }
                return keys;
            }
        }

        public int KeyValue => (int)(keyData & Keys.KeyCode);

        public Keys KeyData => keyData;

        public Keys Modifiers => keyData & Keys.Modifiers;

        public virtual bool Shift => (keyData & Keys.Shift) == Keys.Shift;

        public bool SuppressKeyPress
        {
            get
            {
                return suppressKeyPress;
            }
            set
            {
                suppressKeyPress = value;
                handled = value;
            }
        }

        public KeyEventArgs(Keys keyData)
        {
            this.keyData = keyData;
        }
    }
}