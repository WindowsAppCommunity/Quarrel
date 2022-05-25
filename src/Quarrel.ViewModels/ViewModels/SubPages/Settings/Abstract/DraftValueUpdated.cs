// Quarrel © 2022

using System;

namespace Quarrel.ViewModels.SubPages.Settings.Abstract
{
    /// <summary>
    /// The args of an event raised when a <see cref="DraftValue{T}"/> is updated.
    /// </summary>
    /// <typeparam name="T">The type of value being drafted in the <see cref="DraftValue{T}"/>.</typeparam>
    public class DraftValueUpdated<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DraftValueUpdated{T}"/> class.
        /// </summary>
        internal DraftValueUpdated(T newValue, bool isDraft, bool isDraftChanged)
        {
            NewValue = newValue;
            IsDraft = isDraft;
            IsDraftChanged = isDraftChanged;
        }

        /// <summary>
        /// Gets the new value for the draft value.
        /// </summary>
        public T NewValue { get; }

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
