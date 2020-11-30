using System;
using System.ComponentModel.Composition;

namespace EM.GIS.Controls
{
    /// <summary>
    /// 插件基类
    /// </summary>
    [Serializable]
    public abstract class Plugin : AssemblyInformation, IPlugin
    {
        #region Fields

        private bool _deactivationAllowed = true;

        #endregion

        #region Properties

        [Import]
        public IAppManager App { get; set; }

        public virtual bool DeactivationAllowed
        {
            get
            {
                // Assemblies in the Application Extensions folder cannot be deactivated.
                const string StrApplicationExtensionsDirectoryName = @"\Application Extensions\";
                if (ReferenceAssembly.Location != null && ReferenceAssembly.Location.IndexOf(StrApplicationExtensionsDirectoryName, StringComparison.OrdinalIgnoreCase) >= 0)
                    _deactivationAllowed = false;
                return _deactivationAllowed;
            }

            set
            {
                _deactivationAllowed = value;
            }
        }

        public bool IsActive { get; set; }

        public virtual int Priority => 0;

        #endregion

        #region Methods

        public virtual void Activate()
        {
            IsActive = true;
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }

        #endregion
    }
}
