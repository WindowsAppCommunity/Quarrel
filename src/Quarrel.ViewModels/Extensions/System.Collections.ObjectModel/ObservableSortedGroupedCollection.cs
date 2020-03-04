// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A sorted collection of items, grouped by a key with notification of updates.
    /// </summary>
    /// <typeparam name="TGroup">The type of the key for the groups.</typeparam>
    /// <typeparam name="TType">The type of the values.</typeparam>
    public class ObservableSortedGroupedCollection<TGroup, TType> : ObservableCollection<ObservableGroupCollection<TGroup, TType>>
    {
        private readonly Func<TType, TGroup> _keyReader;
        private readonly Func<TGroup, int> _sorter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableSortedGroupedCollection{TGroup, TType}"/> class.
        /// </summary>
        /// <param name="keyReader">How the key is read from a <typeparamref name="TType"/> value.</param>
        /// <param name="sorter">How the items are ordered.</param>
        public ObservableSortedGroupedCollection(Func<TType, TGroup> keyReader, Func<TGroup, int> sorter)
        {
            _keyReader = keyReader;
            _sorter = sorter;
        }

        /// <summary>
        /// Adds <paramref name="item"/> and group if neccessary.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddElement(TType item)
        {
            CheckReentrancy();
            var group = GetGroupOrCreate(_keyReader.Invoke(item));
            group.Add(item);
        }

        /// <summary>
        /// Adds <paramref name="items"/> and their groups if neccessary.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddElementRange(IEnumerable<TType> items)
        {
            CheckReentrancy();
            var groupedItems = items.GroupBy(_keyReader);
            foreach (var grouping in groupedItems)
            {
                var group = GetGroupOrCreate(grouping.Key);
                group.AddRange(grouping);
            }
        }

        /// <summary>
        /// Removes <paramref name="item"/> from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveElement(TType item)
        {
            CheckReentrancy();
            var group = this.FirstOrDefault(x => x.Key.Equals(item));
            group.Remove(item);
        }

        private ObservableGroupCollection<TGroup, TType> GetGroupOrCreate(TGroup type)
        {
            var group = this.FirstOrDefault(x => x.Key.Equals(type));
            if (group == null)
            {
                var tmp = new ObservableGroupCollection<TGroup, TType>() { Key = type };
                for (int i = 0; i < Count; i++)
                {
                    if (_sorter.Invoke(tmp.Key) < _sorter.Invoke(this[i].Key))
                    {
                        Insert(i, tmp);
                        return tmp;
                    }
                }

                Add(tmp);
                return tmp;
            }

            return group;
        }
    }
}
