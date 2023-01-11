using EM.Bases;
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
    public class Command : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;
        /// <summary>
        /// 执行方法
        /// </summary>
        public Action<object?> ExcuteAction { get; set; }
        /// <summary>
        /// 判断可以执行的方法
        /// </summary>
        public Func<object?,bool> CanExecuteFunc { get; set; }
        public Command()
        { }

        public Command(Action<object?> excute)
        {
            ExcuteAction = excute;
        }
        public Command(Action<object?> excute, Func<object?, bool> canExecute)
        {
            ExcuteAction = excute;
            CanExecuteFunc = canExecute;
        }

        /// <inheritdoc/>
        public virtual bool CanExecute(object? parameter)
        {
            if (CanExecuteFunc == null)
            {
                return true;
            }
            else
            {
                return CanExecuteFunc(parameter);
            }
        }
        /// <inheritdoc/>
        public virtual void Execute(object? parameter)
        {
            if (CanExecute(parameter))
            {
                OnExecute(parameter);
            }
        }
        protected virtual void OnExecute(object? parameter)
        {
            ExcuteAction?.Invoke(parameter);
        }
        /// <summary>
        /// 触发可执行改变事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged(EventArgs.Empty);
        }
        /// <summary>
        /// 可执行改变事件
        /// </summary>
        /// <param name="e">参数</param>
        protected virtual void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteChanged?.Invoke(this, e);
        }
    }
}
