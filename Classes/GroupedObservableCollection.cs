using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Discord_UWP.Managers;

namespace Discord_UWP.Classes
{


    public class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>
    {
        public Grouping(TKey key)
        {
            this.Key = key;
        }

        public Grouping(TKey key, IEnumerable<TElement> items)
            : this(key)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
        
        public TKey Key { get; }
    }

    

    public class MemberComparer : IComparer<Managers.Member>
    {
        public int Compare(Member x, Member y)
        {
            return (x.DisplayName.CompareTo(y.DisplayName));
        }
    }

    /// <summary>
    /// A Grouped yet observable collection.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
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
        public void Clean()
        {
            Clear();
            RoleIndexer.Clear();
        }
        public bool Contains(TElement item)
        {
            return this.Contains(item, (a, b) => a.Equals(b));
        }

        public bool Contains(TElement item, Func<TElement, TElement, bool> compare)
        {
            var key = this.readKey(item);

            var group = this.TryFindGroup(key);
            return group != null && group.Any(i => compare(item, i));
        }

        public IEnumerable<TElement> EnumerateItems()
        {
            return this.SelectMany(g => g);
        }

        public void Add(TElement item)
        {
            var key = this.readKey(item);
            var ogroup = this.FindOrCreateGroup(key);
            Grouping<Managers.HoistRole, Managers.Member> group = ogroup as Grouping<Managers.HoistRole, Managers.Member>;
            (ogroup.Key as HoistRole).Membercount++;
            string DisplayName = (item as Managers.Member).DisplayName;
            
            RoleIndexer.Add((item as Managers.Member).Raw.User.Id, key);
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

        public void ChangeKey(TElement item, TKey previousKey, TKey newKey)
        {
            var previousgroup = this.TryFindGroup(previousKey);
            previousgroup.Remove(item);
            (previousgroup.Key as HoistRole).Membercount--;
            var newgroup = FindOrCreateGroup(newKey);
            RoleIndexer.Remove((item as Managers.Member).Raw.User.Id);
            if (previousgroup != null && previousgroup.Count == 0)
            {
                this.Remove(previousgroup);
                this.LastAffectedGroup = null;
            }
            Add(item);
        }

        public IEnumerable<TKey> Keys => this.Select(i => i.Key);

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

            // var currentItemIndexes = current.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);
            // var replacementItemIndexes = replacement.Select((item, index) => new { item, index }).ToDictionary(i => i.item, i => i.index, itemComparer);

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
                if (this.Count <= i || !this[i].Key.Equals(requiredKeys[i]))
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
                if (keySet.Contains(this[i].Key))
                {
                    this.RemoveAt(i);
                }
            }
        }

        public bool Remove(TElement item)
        {
            
            var key = this.readKey(item);
            var group = this.TryFindGroup(key);
            if(group != null)
                RoleIndexer.Remove((item as Managers.Member).Raw.User.Id);
            var success = group != null && group.Remove(item);

            if (group != null && group.Count == 0)
            {
                this.Remove(group);
                this.LastAffectedGroup = null;
            }
            return success;
        }

        private Grouping<TKey, TElement> TryFindGroup(TKey key)
        {
            if (this.LastAffectedGroup != null && this.LastAffectedGroup.Key.Equals(key))
            {
                return this.LastAffectedGroup;
            }

            var group = this.FirstOrDefault(i => i.Key.Equals(key));
            this.LastAffectedGroup = group;
            return group;
        }

        private Grouping<TKey, TElement> FindOrCreateGroup(TKey key)
        {
            if (this.LastAffectedGroup != null && (this.LastAffectedGroup.Key).Equals(key))
                return this.LastAffectedGroup;

            Grouping<TKey, TElement> result;
            
            foreach (var group in this)
            {
                if (group.Key.Equals(key))
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

                    if ((result.Key as Managers.HoistRole).Position > (this[i].Key as Managers.HoistRole).Position)
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