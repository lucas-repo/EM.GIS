using System;

namespace EM.GIS.Symbology
{
    /// <summary>
    /// Interface for ChangeItem.
    /// </summary>
    public interface IChangeItem
    {
        /// <summary>
        /// Occurs when internal properties or characteristics of this member change.
        /// The member should send itself as the sender of the event.
        /// </summary>
        event EventHandler ItemChanged;

        /// <summary>
        /// An instruction has been sent to remove the specified item from its container.
        /// </summary>
        event EventHandler RemoveItem;
    }
}