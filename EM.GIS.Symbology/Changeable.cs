﻿using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// 可改变类
    /// </summary>
    public class Changeable : IChangeable
    {
        #region Fields

        private bool _changed;
        private bool _ignoreChanges;
        private int _suspendCount;

        #endregion

        /// <inheritdoc/>
        public event EventHandler? Changed;
        /// <inheritdoc/>
        public bool ChangesSuspended => _suspendCount > 0;
        /// <summary>
        /// 触发Changed事件
        /// </summary>
        protected virtual void OnChanged()
        {
            if (_ignoreChanges) return;
            if (ChangesSuspended)
            {
                _changed = true;
                return;
            }
            _ignoreChanges = true;
            Changed?.Invoke(this, EventArgs.Empty);
            _ignoreChanges = false;
        }

        /// <inheritdoc/>
        public virtual void ResumeChanges()
        {
            _suspendCount -= 1;
            if (ChangesSuspended == false)
            {
                if (_changed)
                {
                    OnChanged();
                }
            }
            if (_suspendCount < 0) _suspendCount = 0;
        }

        /// <inheritdoc/>
        public virtual void SuspendChanges()
        {
            if (_suspendCount == 0)
            {
                _changed = false;
            }
            _suspendCount += 1;
        }
    }
}