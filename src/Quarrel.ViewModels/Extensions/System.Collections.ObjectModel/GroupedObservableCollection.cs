// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A grouped yet observable collection.
    /// </summary>
    /// <typeparam name="TKey">They type of the key of each group.</typeparam>
    /// <typeparam name="TElement">The type of elements in each group.</typeparam>
    public sealed class GroupedObservableCollection<TKey, TElement>
        : ObservableCollection<Grouping<TKey, TElement>>
        where TKey : IEquatable<TKey>
        where TElement : class, IEquatable<TElement>
    {
        /// <summary>
        /// The flat list of items in the current instance, for optimization purposes.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        private readonly List<TElement> _elements = new List<TElement>();

        /// <summary>
        /// A function to retrieve the key associated to each item in the collection.
        /// </summary>
        [NotNull]
        private readonly Func<TElement, TKey> _keyReader;

        /// <summary>
        /// Used as optimization for whenever items of a certain group are next to each other.
        /// </summary>
        [CanBeNull]
        private Grouping<TKey, TElement> _lastAffectedGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedObservableCollection{TKey, TElement}"/> class with the specified key selector function.
        /// </summary>
        /// <param name="keyReader">The <see cref="Func{TElement,TKey}"/> to use to assign a group to each item.</param>
        public GroupedObservableCollection([NotNull] Func<TElement, TKey> keyReader) => _keyReader = keyReader;

        /// <summary>
        /// Gets the sequence of keys in the current instance.
        /// </summary>
        [NotNull]
        public IEnumerable<TKey> Keys
        {
            [Pure]
            [LinqTunnel]
            get => this.Select(i => i.Key);
        }

        /// <summary>
        /// Gets the sequence of elements in the current instance.
        /// </summary>
        [NotNull]
        [ItemNotNull]
        public IReadOnlyList<TElement> Elements => _elements;

        /// <summary>
        /// Gets whether or not the given element is currently present in the collection.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>True if the <see cref="GroupedObservableCollection{TKey, TElement}"/> contains <paramref name="item"/>.</returns>
        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public bool Contains([NotNull] TElement item)
        {
            TKey key = _keyReader(item);
            Grouping<TKey, TElement> group = TryFindGroup(key);
            return group?.Contains(item) ?? false;
        }

        /// <summary>
        /// Adds a new item in the grouping collection.
        /// </summary>
        /// <param name="item">The new item to add to the grouping collection.</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public void AddElement([NotNull] TElement item)
        {
            // Get the target group for the new item
            TKey key = _keyReader(item);
            Grouping<TKey, TElement> group = FindOrCreateGroup(key);

            // Add to end of list
            _elements.Add(item);
            group.Add(item);
        }

        /// <summary>
        /// Removes a given item from the grouped collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>Whether or not the item was in the collection.</returns>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public bool Remove([NotNull] TElement item)
        {
            // Try to get the target group
            TKey key = _keyReader(item);
            if (!(TryFindGroup(key) is Grouping<TKey, TElement> group))
            {
                return false;
            }

            // Remove the item from the group, and remove the group if it's empty
            _elements.Remove(item);
            group.Remove(item);
            if (group.Count == 0)
            {
                Remove(group);
                _lastAffectedGroup = null;
            }

            return true;
        }

        /// <summary>
        /// This fully clears and resets the member list, including the optimization-related objects.
        /// </summary>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public new void Clear()
        {
            base.Clear();
            _elements.Clear();
            _lastAffectedGroup = null;
        }

        /// <summary>
        /// Tries to find the group with the given key in the current collection.
        /// </summary>
        /// <param name="key">The key of the group to look for.</param>
        [MustUseReturnValue]
        [CanBeNull]
        [CollectionAccess(CollectionAccessType.Read)]
        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            // Check the last accessed group first
            if (_lastAffectedGroup?.Key.Equals(key) == true)
            {
                return _lastAffectedGroup;
            }

            // Try to get the target group
            Grouping<TKey, TElement> group = this.FirstOrDefault(i => i.Key.Equals(key));
            if (group != null)
            {
                _lastAffectedGroup = group;
            }

            return group;
        }

        /// <summary>
        /// Gets or creates a group with the specified key.
        /// </summary>
        /// <param name="key">The key of the group to find or create.</param>
        [MustUseReturnValue]
        [NotNull]
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            // Check the last accessed group first
            if (_lastAffectedGroup?.Key.Equals(key) == true)
            {
                return _lastAffectedGroup;
            }

            // Check if the target group already exists
            if (this.FirstOrDefault(g => g.Key.Equals(key)) is Grouping<TKey, TElement> group)
            {
                return _lastAffectedGroup = group;
            }

            // The group doesn't exist already, create a new one in the correct position
            Grouping<TKey, TElement> result = _lastAffectedGroup = new Grouping<TKey, TElement>(key);

            // Add in the last position
            Add(result);
            return result;
        }
    }
}
