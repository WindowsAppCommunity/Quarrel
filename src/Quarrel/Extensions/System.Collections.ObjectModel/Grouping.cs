using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A custom <see cref="ObservableCollection{T}"/> that implements the <see cref="IGrouping{TKey,TElement}"/> <see langword="interface"/>
    /// </summary>
    /// <typeparam name="TKey">They type of the key of each group</typeparam>
    /// <typeparam name="TElement">The type of elements in each group</typeparam>
    public sealed class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
    {
        /// <summary>
        /// Gets the key associated with the current group
        /// </summary>
        public TKey Key { get; }

        /// <summary>
        /// Creates a new <see cref="Grouping{TKey, TElement}"/> instance with the specified key
        /// </summary>
        /// <param name="key">They key to use in the new instance</param>
        public Grouping(TKey key) => Key = key;

        /// <summary>
        /// Creates a new <see cref="Grouping{TKey, TElement}"/> instance with the specified key and items collection
        /// </summary>
        /// <param name="key">The key to use in the new instance</param>
        /// <param name="items">The initial collection of items to insert in the collection</param>
        public Grouping(TKey key, [NotNull, ItemNotNull] IEnumerable<TElement> items) : this(key)
        {
            foreach (TElement item in items) Add(item);
        }
    }
}
