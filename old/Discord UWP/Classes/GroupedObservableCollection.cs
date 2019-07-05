using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Quarrel.Managers;
using Quarrel.SimpleClasses;

namespace Quarrel.Classes
{
    /// <summary>
    /// Group of items for <see cref="GroupedObservableCollection{TKey, TElement}"/> collection
    /// </summary>
    /// <typeparam name="TKey">The group data</typeparam>
    /// <typeparam name="TElement">Individual item data</typeparam>
    public class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
    {
        /// <summary>
        /// Initialize with grouping data to add contents later
        /// </summary>
        /// <param name="key">Grouping data</param>
        public Grouping(TKey key)
        {
            this.Group = key;
        }

        /// <summary>
        /// Initalize with grouping data and contents
        /// </summary>
        /// <param name="key">Grouping data</param>
        /// <param name="items">Collection items</param>
        public Grouping(TKey key, IEnumerable<TElement> items)
            : this(key)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
        
        public TKey Group { get; }
    }

    /// <summary>
    /// Used for getting the order of two <seealso cref="Member"/> items
    /// </summary>
    public class MemberComparer : IComparer<Member>
    {
        /// <summary>
        /// Compares two <seealso cref="Member"/> items
        /// </summary>
        /// <param name="x">First item to compare</param>
        /// <param name="y">Second item to compare</param>
        /// <returns>An indication wheather item x follows, proceeds or is equal in position to y</returns>
        public int Compare(Member x, Member y)
        {
            return (x.DisplayName.CompareTo(y.DisplayName));
        }
    }

    /// <summary>
    /// A Grouped yet observable collection.
    /// </summary>
    /// <typeparam name="TKey">Grouping type</typeparam>
    /// <typeparam name="TElement">Item types</typeparam>
    public class GroupedObservableCollection<TKey, TElement> : ObservableCollection<Grouping<TKey, TElement>>
        where TKey : IComparable<TKey>
    {
        private readonly Func<TElement, TKey> readKey;

        /// <summary>
        /// Used as optimization for whenever items of a certain group are next to each other
        /// </summary>
        private Grouping<TKey, TElement> LastAffectedGroup;
        public Dictionary<string, TKey> RoleIndexer = new Dictionary<string, TKey>();
        public GroupedObservableCollection(Func<TElement, TKey> readKey)
        {
            this.readKey = readKey;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
        }

        public GroupedObservableCollection(Func<TElement, TKey> readKey, IEnumerable<TElement> items)
            : this(readKey)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// This fully clears and resets the member list, including the optimization-related objects
        /// </summary>
        public void Clean()
        {
            Clear();
            RoleIndexer.Clear();
        }

        /// <summary>
        /// Checks if Collection contains item
        /// </summary>
        /// <param name="item">item to check for</param>
        /// <returns>Wheather or not the item is in the collection</returns>
        public bool Contains(TElement item)
        {
            return this.Contains(item, (a, b) => a.Equals(b));
        }

        /// <summary>
        /// Checks if Collection contains item according to <paramref name="compare"/> function
        /// </summary>
        /// <param name="item">Item to check if contained</param>
        /// <param name="compare">Function to compare with</param>
        /// <returns>Wheather or not the collection contains the item</returns>
        public bool Contains(TElement item, Func<TElement, TElement, bool> compare)
        {
            var key = this.readKey(item);

            var group = this.TryFindGroup(key);
            return group != null && group.Any(i => compare(item, i));
        }

        /// <summary>
        /// Get the collection as <seealso cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns>IEnumberable of collection items</returns>
        public IEnumerable<TElement> EnumerateItems()
        {
            return this.SelectMany(g => g);
        }

        /// <summary>
        /// Add <paramref name="item"/> to collection
        /// </summary>
        /// <param name="item"><seealso cref="TElement"/> to add</param>
        public void Add(TElement item)
        {
            var key = this.readKey(item);

            // Give the item a group
            var ogroup = this.FindOrCreateGroup(key);
            
            // No longer so dynamic...
            var member = (item as Member);
            Grouping<HoistRole, Member> group = ogroup as Grouping<HoistRole, Member>;
            (ogroup.Group as HoistRole).Membercount++;
            string DisplayName = member.DisplayName;
            
            if(!RoleIndexer.ContainsKey(member.Raw.User.Id))
                RoleIndexer.Add(member.Raw.User.Id, key);

            //try to insert logically
            for(var i = 0; i < group.Count; i++)
            {
                int compare = group[i].DisplayName.CompareTo(DisplayName);
                if (compare > 0)
                {
                    ogroup.Insert(i, item);
                    return;
                }
            }
            //if it doesn't get added, just add normally
            ogroup.Add(item);
        }

        /// <summary>
        /// Change the Group of an item
        /// </summary>
        /// <param name="item">Item to change group of</param>
        /// <param name="previousKey">The previous group</param>
        /// <param name="newKey">The new group</param>
        public void ChangeKey(TElement item, TKey previousKey, TKey newKey)
        {
            // Remove from old group
            var previousgroup = this.TryFindGroup(previousKey);
            previousgroup.Remove(item);
            (previousgroup.Group as HoistRole).Membercount--;
            var newgroup = FindOrCreateGroup(newKey);
            RoleIndexer.Remove((item as Member).Raw.User.Id);
            if (previousgroup != null && previousgroup.Count == 0)
            {
                this.Remove(previousgroup);
                this.LastAffectedGroup = null;
            }

            // Add to new group
            Add(item);
        }

        /// <summary>
        /// List of groups in Collection
        /// </summary>
        public IEnumerable<TKey> Keys => this.Select(i => i.Group);

        /// <summary>
        /// Swap the collection with another collection
        /// </summary>
        /// <param name="replacementCollection">New collection</param>
        /// <param name="itemComparer">Comparer used on items of that collection</param>
        /// <returns>New collection</returns>
        public GroupedObservableCollection<TKey, TElement> ReplaceWith(GroupedObservableCollection<TKey, TElement> replacementCollection, IEqualityComparer<TElement> itemComparer)
        {
            // First make sure that the top level group containers match
            var replacementKeys = replacementCollection.Keys.ToList();
            var currentKeys = new HashSet<TKey>(this.Keys);
            this.RemoveGroups(currentKeys.Except(replacementKeys));
            this.EnsureGroupsExist(replacementKeys);

            Debug.Assert(this.Keys.SequenceEqual(replacementCollection.Keys), "Expected this collection to have exactly the same keys in the same order at this point");

            for (var i = 0; i < replacementCollection.Count; i++)
            {
                MergeGroup(this[i], replacementCollection[i], itemComparer);
            }

            return this;
        }
        
        private static void MergeGroup(Grouping<TKey, TElement> current, Grouping<TKey, TElement> replacement, IEqualityComparer<TElement> itemComparer)
        {
            // Shortcut the matching and reordering process if the sequences are the same
            if (current.SequenceEqual(replacement, itemComparer))
            {
                return;
            }

            // First remove any items that are not present in the replacement
            var resultSet = new HashSet<TElement>(replacement, itemComparer);
            foreach (var toRemove in current.Except(resultSet, itemComparer).ToList())
            {
                current.Remove(toRemove);
            }

            Debug.Assert(new HashSet<TElement>(current, itemComparer).IsSubsetOf(replacement), "Expected the current group to be a subset of the replacement group");

            var currentItemSet = new HashSet<TElement>(current, itemComparer);
            for (var i = 0; i < replacement.Count; i++)
            {
                var findElement = replacement[i];

                if (i >= current.Count || !itemComparer.Equals(current[i], findElement))
                {
                    if (currentItemSet.Contains(findElement))
                    {
                        // The current set already contains the item, but it's in a different position
                        // Find out where it is in the current collection and move it from there
                        // NOTE this isn't very optimal if large sets are being reordered a lot, but the general use case for
                        // this sort of list is that there is some inherent order that won't be changing.
                        var moved = false;
                        for (var j = i + 1; j < current.Count; j++)
                        {
                            if (itemComparer.Equals(current[j], findElement))
                            {
                                current.Move(i, j);
                                moved = true;
                                break;
                            }
                        }

                        Debug.Assert(moved, "Expected that the element should have been moved");
                    }
                    else
                    {
                        // This is a new element, insert it here
                        current.Insert(i, replacement[i]);
                    }
                }
            }
        }

        private void EnsureGroupsExist(IList<TKey> requiredKeys)
        {
            Debug.Assert(new HashSet<TKey>(this.Keys).IsSubsetOf(requiredKeys), "Expected this collection to contain no additional keys than the new collection at this point");

            if (this.Count == requiredKeys.Count)
            {
                // Nothing to do.
                return;
            }

            for (var i = 0; i < requiredKeys.Count; i++)
            {
                if (this.Count <= i || !this[i].Group.Equals(requiredKeys[i]))
                {
                    this.Insert(i, new Grouping<TKey, TElement>(requiredKeys[i]));
                }
            }
        }

        private void RemoveGroups(IEnumerable<TKey> keys)
        {
            var keySet = new HashSet<TKey>(keys);
            for (var i = this.Count - 1; i >= 0; i--)
            {
                if (keySet.Contains(this[i].Group))
                {
                    this.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Removes an item from the collection
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <returns>Wheather or not the item was removed</returns>
        public bool Remove(TElement item)
        {
            
            var key = this.readKey(item);
            var group = this.TryFindGroup(key);
            if(group != null)
                RoleIndexer.Remove((item as Member).Raw.User.Id);
            var success = group != null && group.Remove(item);

            if (group != null && group.Count == 0)
            {
                this.Remove(group);
                this.LastAffectedGroup = null;
            }
            return success;
        }

        /// <summary>
        /// Gets a grouping if it exists
        /// </summary>
        /// <param name="key">Group data of grouping</param>
        /// <returns>Found Grouping or null if not present</returns>
        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            if (this.LastAffectedGroup != null && this.LastAffectedGroup.Group.Equals(key))
            {
                return this.LastAffectedGroup;
            }

            var group = this.FirstOrDefault(i => i.Group.Equals(key));
            this.LastAffectedGroup = group;
            return group;
        }

        /// <summary>
        /// Gets a grouping and creates one if it doesn't yet exist
        /// </summary>
        /// <param name="key">The group data to get a grouping for</param>
        /// <returns>The grouping either found or created</returns>
        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            if (this.LastAffectedGroup != null && (this.LastAffectedGroup.Group).Equals(key))
                return this.LastAffectedGroup;

            Grouping<TKey, TElement> result;
            
            foreach (var group in this)
            {
                if (group.Group.Equals(key))
                {
                    result = group;
                    this.LastAffectedGroup = result;
                    return result;
                } 
            }
            // Code got this far, so group doesn't exist and the new group needs to be created and be positioned wherever, based on it's position property
            result = new Grouping<TKey, TElement>(key);
            this.LastAffectedGroup = result;
            bool assigned = false;
            if(key != null)
            {
                for (var i = 0; i < this.Count; i++)
                {

                    if ((result.Group as HoistRole).Position > (this[i].Group as HoistRole).Position)
                    {
                        //Loop until a group has a position which is higher, then insert the new group just before
                        this.Insert(i, result);
                        return result;
                    }
                }
            }

            if(!assigned) //No groups have been added yet, or none were higher, so just add the new group to the end
                this.Add(result);
            return result;
        }
    }

}