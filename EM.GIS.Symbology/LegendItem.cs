using EM.Bases;
using EM.GIS.Data;
using System;
using System.Collections.ObjectModel;
using System.Drawing;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 图例元素
    /// </summary>
    [Serializable]
    public abstract class LegendItem : TreeItem, ILegendItem
    {
        public ObservableCollection<IBaseCommand> ContextCommands { get; } = new ObservableCollection<IBaseCommand>();

        private ProgressDelegate _progress;
        public ProgressDelegate Progress
        {
            get { return _progress; }
            set
            {
                if (SetProperty(ref _progress, value, nameof(ProgressHandler)))
                {
                    foreach (var item in Children)
                    {
                        if (item is IProgressHandler handler)
                        {
                            handler.Progress=_progress;
                        }
                    }
                }
            }
        }

        public LegendItem()
        {
        }
        public LegendItem(ILegendItem parent) : this()
        {
            Parent = parent;
        }

        public virtual void DrawLegend(Graphics g, Rectangle rectangle) { }
    }
}