using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A grouped yet observable collection
    /// </summary>
    /// <typeparam name="TKey">They type of the key of each group</typeparam>
    /// <typeparam name="TElement">The type of elements in each group</typeparam>
    public sealed class GroupedObservableCollection<TKey, TElement>
        : ObservableCollection<Grouping<TKey, TElement>>
        where TKey : IEquatable<TKey>, IComparable<TKey>
        where TElement : class, IEquatable<TElement>, IComparable<TElement>
    {
        #region Properties

        /// <summary>
        /// Gets the sequence of keys in the current instance
        /// </summary>
        [NotNull]
        public IEnumerable<TKey> Keys
        {
            [Pure, LinqTunnel]
            get => this.Select(i => i.Key);
        }

        // The flat list of items in the current instance, for optimization purposes
        [NotNull, ItemNotNull]
        private readonly List<TElement> _Elements = new List<TElement>();

        /// <summary>
        /// Gets the sequence of elements in the current instance
        /// </summary>
        [NotNull, ItemNotNull]
        public IReadOnlyList<TElement> Elements => _Elements;

        #endregion

        #region Fields and initialization

        /// <summary>
        /// A function to retrieve the key associated to each item in the collection
        /// </summary>
        [NotNull]
        private readonly Func<TElement, TKey> KeyReader;

        /// <summary>
        /// Used as optimization for whenever items of a certain group are next to each other
        /// </summary>
        [CanBeNull]
        private Grouping<TKey, TElement> _LastAffectedGroup;

        /// <summary>
        /// Creates a new <see cref="GroupedObservableCollection{TKey,TElement}"/> instance with the specified key selector function
        /// </summary>
        /// <param name="keyReader">The <see cref="Func{TElement,TKey}"/> to use to assign a group to each item</param>
        public GroupedObservableCollection([NotNull] Func<TElement, TKey> keyReader) => KeyReader = keyReader;

        #endregion

        #region Public methods

        /// <summary>
        /// Gets whether or not the given element is currently present in the collection
        /// </summary>
        /// <param name="item">The item to look for</param>
        [Pure]
        [CollectionAccess(CollectionAccessType.Read)]
        public TElement TryGet([NotNull] TElement item)
        {
            TKey key = KeyReader(item);
            Grouping<TKey, TElement> group = TryFindGroup(key);
            return group?.FirstOrDefault(i => i.Equals(item));
        }

        /// <summary>
        /// Adds a new item in the grouping collection
        /// </summary>
        /// <param name="item">The new item to add to the grouping collection</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public void Add([NotNull] TElement item)
        {
            // Get the target group for the new item
            TKey key = KeyReader(item);
            Grouping<TKey, TElement> group = FindOrCreateGroup(key);

            // Insert the item in the right position in the group
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i].CompareTo(item) > 0)
                {
                    _Elements.Add(item);
                    group.Insert(i, item);
                    return;
                }
            }

            _Elements.Add(item);
            group.Add(item);
        }

        /// <summary>
        /// Removes a given item from the grouped collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public bool Remove([NotNull] TElement item)
        {
            // Try to get the target group
            TKey key = KeyReader(item);
            if (!(TryFindGroup(key) is Grouping<TKey, TElement> group)) return false;

            // Remove the item from the group, and remove the group if it's empty
            _Elements.Remove(item);
            group.Remove(item);
            if (group.Count == 0)
            {
                Remove(group);
                _LastAffectedGroup = null;
            }

            return true;
        }

        /// <summary>
        /// This fully clears and resets the member list, including the optimization-related objects
        /// </summary>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public new void Clear()
        {
            base.Clear();
            _Elements.Clear();
            _LastAffectedGroup = null;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Tries to find the group with the given key in the current collection
        /// </summary>
        /// <param name="key">The key of the group to look for</param>
        [MustUseReturnValue, CanBeNull]
        [CollectionAccess(CollectionAccessType.Read)]
        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            // Check the last accessed group first
            if (_LastAffectedGroup?.Key.Equals(key) == true)
                return _LastAffectedGroup;

            // Try to get the target group
            Grouping<TKey, TElement> group = this.FirstOrDefault(i => i.Key.Equals(key));
            if (group != null) _LastAffectedGroup = group;
            return group;
        }

        /// <summary>
        /// Gets or creates a group with the specified key
        /// </summary>
        /// <param name="key">The key of the group to find or create</param>
        [MustUseReturnValue, NotNull]
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            // Check the last accessed group first
            if (_LastAffectedGroup?.Key.Equals(key) == true)
                return _LastAffectedGroup;

            // Check if the target group already exists
            if (this.FirstOrDefault(g => g.Key.Equals(key)) is Grouping<TKey, TElement> group)
            {
                return _LastAffectedGroup = group;
            }

            // The group doesn't exist already, create a new one in the correct position
            Grouping<TKey, TElement> result = _LastAffectedGroup = new Grouping<TKey, TElement>(key);
            for (var i = 0; i < Count; i++)
            {
                if (result.Key.CompareTo(this[i].Key) > 0)
                {
                    Insert(i, result);
                    return result;
                }
            }

            // Add in the last position if necessary
            Add(result);
            return result;
        }

        #endregion
    }
}
