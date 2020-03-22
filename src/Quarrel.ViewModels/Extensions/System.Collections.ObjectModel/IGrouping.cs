// Copyright (c) Quarrel. All rights reserved.

namespace Quarrel.ViewModels.Extensions.System.Collections.ObjectModel
{
    /// <summary>
    /// A Group of items.
    /// </summary>
    public interface IGrouping
    {
        /// <summary>
        /// Gets the key for the group.
        /// </summary>
        object Key { get; }

        /// <summary>
        /// Gets the amount of items in the group.
        /// </summary>
        int Count { get; }
    }
}
