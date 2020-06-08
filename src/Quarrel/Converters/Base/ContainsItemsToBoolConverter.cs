// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace Quarrel.Converters.Base
{
    /// <summary>
    /// A converter that returns a <see cref="bool"/> indicating whether or not the collection contains any items.
    /// </summary>
    public class ContainsItemsToBoolConverter
    {
        /// <summary>
        /// Checks if a collection contains any items.
        /// </summary>
        /// <param name="collection">The collection to check.</param>
        /// <returns>Whether or not the collection contains any items.</returns>
        public static bool Convert(IEnumerable<object> collection)
        {
            return collection != null && collection.Any();
        }
    }
}
