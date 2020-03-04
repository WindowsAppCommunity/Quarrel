// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    public interface ObservableGrouping
    {
        object Group { get; }
        int Count { get; }
    }

    public class ObservableGroupCollection<key, value> : ObservableRangeCollection<value>, IGrouping<key, value>, ObservableGrouping
    {
        public key Key { get; set; }
        public object Group => Key;
    }

    public class ObservableSortedGroupedCollection<Group, Type> : ObservableCollection<ObservableGroupCollection<Group, Type>>
    {
        private readonly Func<Type, Group> KeyReader;
        private readonly Func<Group, int> Sorter;

        public ObservableSortedGroupedCollection(Func<Type, Group> keyReader, Func<Group, int> sorter)
        {
            KeyReader = keyReader;
            Sorter = sorter;
        }

        public void AddElement(Type item)
        {
            CheckReentrancy();
            var group = GetGroupOrCreate(KeyReader.Invoke(item));
            group.Add(item);
        }

        public void AddElementRange(IEnumerable<Type> item)
        {
            CheckReentrancy();
            var groupedItems = item.GroupBy(KeyReader);
            foreach (var grouping in groupedItems)
            {
                var group = GetGroupOrCreate(grouping.Key);
                group.AddRange(grouping);
            }
        }
        public void RemoveElement(Type item)
        {
            CheckReentrancy();
            var group = this.FirstOrDefault(x => x.Key.Equals(item));
            group.Remove(item);
        }

        private ObservableGroupCollection<Group, Type> GetGroupOrCreate(Group type)
        {
            var group = this.FirstOrDefault(x => x.Key.Equals(type));
            if (group == null)
            {
                var tmp = new ObservableGroupCollection<Group, Type>() { Key = type };
                for (int i = 0; i < Count; i++)
                {
                    if (Sorter.Invoke(tmp.Key) < Sorter.Invoke(this[i].Key))
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
