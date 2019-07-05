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
    public class HashedGrouping<TKey, TGroup, TElement> : ObservableHashedCollection<TKey, TElement>, IGrouping<TGroup, TElement>, HashedGrouping
    {
        /// <summary>
        /// Gets the key associated with the current group
        /// </summary>
        public TGroup Key { get; }

        object HashedGrouping.Group => Key;

        /// <summary>
        /// Creates a new <see cref="Grouping{TKey, TElement}"/> instance with the specified key and items collection
        /// </summary>
        /// <param name="group">The key to use in the new instance</param>
        /// <param name="items">The initial collection of items to insert in the collection</param>
        public HashedGrouping(TGroup group, [NotNull, ItemNotNull] ICollection<KeyValuePair<TKey, TElement>> items) : base(items)
        {
            Key = group;
        }
    }
    public interface HashedGrouping
    {
        object Group { get; }
        int Count { get; }

    }
}
