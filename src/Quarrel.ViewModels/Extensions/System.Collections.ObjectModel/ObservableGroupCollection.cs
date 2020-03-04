// Copyright (c) Quarrel. All rights reserved.

using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A group from an <see cref="ObservableSortedGroupedCollection{TGroup, TType}"/>.
    /// </summary>
    /// <typeparam name="TKey">The key type for the <see cref="ObservableSortedGroupedCollection{TGroup, TType}"/>.</typeparam>
    /// <typeparam name="TValue">The values in the <see cref="ObservableSortedGroupedCollection{TGroup, TType}"/>.</typeparam>
    public class ObservableGroupCollection<TKey, TValue> : ObservableRangeCollection<TValue>, IGrouping<TKey, TValue>, IObservableGrouping
    {
        /// <summary>
        /// Gets or sets the key for this Group.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// Gets the <see cref="Key"/> as an object.
        /// </summary>
        public object Group => this.Key;
    }
}
