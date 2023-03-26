using EM.WpfBases;
using System;
using System.Windows;
using System.Windows.Threading;

namespace EM.GIS.Tools
{
    /// <summary>
    /// 有进度的视图模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportableViewModel<T> : ViewModel<T>, IReportable where T : FrameworkElement
    {
        public ReportableViewModel(T t) : base(t)
        {
        }
        private bool _isFree = true;
        /// <inheritdoc/>
        public bool IsFree
        {
            get => _isFree;
            set => Dispatcher.CurrentDispatcher.Invoke(() => SetProperty(ref _isFree, value));
        }
        public Action<string, int> ProgressAction { get; set; }

        public virtual void Cancel()
        {
        }
    }
}