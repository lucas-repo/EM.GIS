using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EM.GIS.WPFControls
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public abstract class Command : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;
        private Action<object?> _excute;
        private Func<object?,bool> _canExecute;
        public Command()
        { }

        protected Command(Action<object?> excute, Func<object?, bool> canExecute)
        {
            _excute = excute;
            _canExecute = canExecute;
        }

        /// <inheritdoc/>
        public virtual bool CanExecute(object? parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }
            else
            {
                return _canExecute(parameter);
            }
        }

        /// <inheritdoc/>
        public virtual void Execute(object? parameter)
        {
            _excute?.Invoke(parameter);
        }
    }
}
