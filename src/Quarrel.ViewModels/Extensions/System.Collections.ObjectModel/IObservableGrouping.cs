// Copyright (c) Quarrel. All rights reserved.

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A grouping in an <see cref="ObservableSortedGroupedCollection{TGroup, TType}"/>.
    /// </summary>
    public interface IObservableGrouping
    {
        /// <summary>
        /// Gets the Key for the group.
        /// </summary>
        object Group { get; }

        /// <summary>
        /// Gets the number of items in the group.
        /// </summary>
        int Count { get; }
    }
}
