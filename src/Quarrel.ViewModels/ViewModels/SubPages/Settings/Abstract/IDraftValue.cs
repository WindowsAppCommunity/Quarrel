// Quarrel © 2022

using System;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// An interface for a value that can have an updated draft.
    /// </summary>
    public interface IDraftValue
    {
        /// <summary>
        /// An event raised when the value is updated.
        /// </summary>
        event EventHandler<DraftValueUpdated>? ValueChanged;

        /// <summary>
        /// Applies the value to the canonical value.
        /// </summary>
        void Apply();

        /// <summary>
        /// Resets the value to the canonical value.
        /// </summary>
        void Reset();
    }
}
