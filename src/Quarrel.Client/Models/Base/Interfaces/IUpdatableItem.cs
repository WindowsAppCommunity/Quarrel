// Quarrel © 2022

using System;

namespace Quarrel.Client.Models.Base.Interfaces
{
    /// <summary>
    /// An interface for 
    /// </summary>
    public interface IUpdatableItem
    {
        /// <summary>
        /// An event invoked when the item is updated.
        /// </summary>
        public event EventHandler? ItemUpdated;
    }
}
