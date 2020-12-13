using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace EM.GIS
{
    /// <summary>
    /// 委托命令类
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// 执行命令委托
        /// </summary>
        public Action<object> ExecuteCommand { get; set; }
        /// <summary>
        /// 命令是否可用委托
        /// </summary>
        public Func<object, bool> CanExecuteCommand { get; set; }
        /// <summary>
        /// 命令是否可用改变事件
        /// </summary>
        public event EventHandler CanExecuteChanged;
        public DelegateCommand()
        { }
        /// <summary>
        /// 实例化委托命令类
        /// </summary>
        /// <param name="executeCommand">执行的方法委托</param>
        /// <param name="canExecuteCommand">计算能否执行的方法委托</param>
        /// <param name="canExecuteChanged">能否执行改变后的委托</param>
        public DelegateCommand(Action<object> executeCommand, Func<object, bool> canExecuteCommand = null, EventHandler canExecuteChanged = null)
        {
            ExecuteCommand = executeCommand;
            CanExecuteCommand = canExecuteCommand;
            CanExecuteChanged = canExecuteChanged;
        }
        /// <summary>
        /// 返回命令是否可用
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <returns>可用为true，反之false</returns>
        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
            {
                return CanExecuteCommand(parameter);
            }
            return true;
        }
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameter">参数</param>
        public void Execute(object parameter)
        {
            ExecuteCommand?.Invoke(parameter);
        }
        /// <summary>
        /// 调用计算命令是否可用改变事件
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
