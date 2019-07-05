using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A grouped yet observable collection
    /// </summary>
    /// <typeparam name="TGroup">They type of the key of each group</typeparam>
    /// <typeparam name="TElement">The type of elements in each group</typeparam>
    public sealed class GroupedObservableHashedCollection<TKey, TGroup, TElement>
        : ObservableHashedCollection<TKey, HashedGrouping<TKey, TGroup, TElement>>
        where TGroup : IEquatable<TGroup>, IComparable<TGroup>
        where TElement : class, IEquatable<TElement>, IComparable<TElement>
    {
        #region Properties

        /// <summary>
        /// Gets the sequence of keys in the current instance
        /// </summary>
        [NotNull]
        public IEnumerable<TGroup> Groups
        {
            [Pure, LinqTunnel]
            get => this.Values.Select(i => i.Key);
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
        private readonly Func<TElement, TGroup> KeyReader;

        /// <summary>
        /// Used as optimization for whenever items of a certain group are next to each other
        /// </summary>
        [CanBeNull]
        private HashedGrouping<TKey, TGroup, TElement> _LastAffectedGroup;

        /// <summary>
        /// Creates a new <see cref="GroupedObservableCollection{TKey,TElement}"/> instance with the specified key selector function
        /// </summary>
        /// <param name="keyReader">The <see cref="Func{TElement,TKey}"/> to use to assign a group to each item</param>
        public GroupedObservableHashedCollection([NotNull] Func<TElement, TGroup> keyReader, ICollection<KeyValuePair<TKey, HashedGrouping<TKey, TGroup, TElement>>> collection) : base(collection) 
        {
            KeyReader = keyReader;
        }

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
            TGroup key = KeyReader(item);
            HashedGrouping<TKey, TGroup, TElement> group = TryFindGroup(key);
            return group?.Values.FirstOrDefault(i => i.Equals(item));
        }

        /// <summary>
        /// Adds a new item in the grouping collection
        /// </summary>
        /// <param name="item">The new item to add to the grouping collection</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public void AddElement(TKey key, [NotNull] TElement item)
        {
            // Get the target group for the new item
            TGroup group = KeyReader(item);
            HashedGrouping<TKey, TGroup, TElement> hashGroup = FindOrCreateGroup(key, group);
            Add(key, hashGroup);

            // Insert the item in the right position in the group
            for (int i = 0; i < hashGroup.Count; i++)
            {
                if (hashGroup.Values.ElementAt(i).CompareTo(item) > 0)
                {
                    _Elements.Add(item);
                    hashGroup.Values.Add(item);
                    return;
                }
            }

            _Elements.Add(item);
            hashGroup.Add(key, item);
        }

        /// <summary>
        /// Removes a given item from the grouped collection
        /// </summary>
        /// <param name="item">The item to remove</param>
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        public bool Remove([NotNull] TKey key)
        {
            TElement item = this[key][key];
            // Try to get the target group
            TGroup group = KeyReader(item);
            if (!(TryFindGroup(group) is HashedGrouping<TKey, TGroup, TElement> hashedGroup)) return false;

            // Remove the item from the group, and remove the group if it's empty
            _Elements.Remove(item);
            hashedGroup.Remove(key);
            if (hashedGroup.Count == 0)
            {
                Remove(key);
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
        private HashedGrouping<TKey, TGroup, TElement> TryFindGroup(TGroup key)
        {
            // Check the last accessed group first
            if (_LastAffectedGroup?.Key.Equals(key) == true)
                return _LastAffectedGroup;

            // Try to get the target group
            HashedGrouping<TKey, TGroup, TElement> group = this.Values.FirstOrDefault(i => i.Key.Equals(key));
            if (group != null) _LastAffectedGroup = group;
            return group;
        }

        /// <summary>
        /// Gets or creates a group with the specified key
        /// </summary>
        /// <param name="group">The key of the group to find or create</param>
        [MustUseReturnValue, NotNull]
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        private HashedGrouping<TKey, TGroup, TElement> FindOrCreateGroup(TKey key, TGroup group)
        {
            // Check the last accessed group first
            if (_LastAffectedGroup?.Key.Equals(group) == true)
                return _LastAffectedGroup;

            // Check if the target group already exists
            if (this.Values.FirstOrDefault(g => g.Key.Equals(group)) is HashedGrouping<TKey, TGroup, TElement> hashGroup)
            {
                return _LastAffectedGroup = hashGroup;
            }

            // The group doesn't exist already, create a new one in the correct position
            HashedGrouping<TKey, TGroup, TElement> result = _LastAffectedGroup = new HashedGrouping<TKey, TGroup, TElement>(group, new List<KeyValuePair<TKey, TElement>>());
            for (var i = 0; i < Count; i++)
            {
                if (result.Key.CompareTo(this.Values.ElementAt(i).Key) > 0)
                {
                    Add(key, result);
                    return result;
                }
            }

            // Add in the last position if necessary
            Add(key, result);
            return result;
        }

        #endregion
    }
}
