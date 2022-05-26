// Quarrel © 2022

using System;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// The args of an event raised when a <see cref="DraftValue{T}"/> is updated.
    /// </summary>
    public class DraftValueUpdated : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DraftValueUpdated"/> class.
        /// </summary>
        internal DraftValueUpdated(bool isDraft, bool isDraftChanged)
        {
            IsDraft = isDraft;
            IsDraftChanged = isDraftChanged;
        }

        /// <summary>
        /// Gets whether or not the new value is a draft.
        /// </summary>
        public bool IsDraft { get; }

        /// <summary>
        /// Gets whether or not the updated value changed the draft status of the <see cref="DraftValue{T}"/>.
        /// </summary>
        public bool IsDraftChanged { get; }
    }
}
