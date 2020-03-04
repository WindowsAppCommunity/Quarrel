// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A grouping 
    /// </summary>
    public interface IObservableGrouping
    {
        object Group { get; }

        int Count { get; }
    }

    public class ObservableSortedGroupedCollection<TGroup, TType> : ObservableCollection<ObservableGroupCollection<TGroup, TType>>
    {
        private readonly Func<TType, TGroup> KeyReader;
        private readonly Func<TGroup, int> Sorter;

        public ObservableSortedGroupedCollection(Func<TType, TGroup> keyReader, Func<TGroup, int> sorter)
        {
            KeyReader = keyReader;
            Sorter = sorter;
        }

        public void AddElement(TType item)
        {
            CheckReentrancy();
            var group = GetGroupOrCreate(KeyReader.Invoke(item));
            group.Add(item);
        }

        public void AddElementRange(IEnumerable<TType> item)
        {
            CheckReentrancy();
            var groupedItems = item.GroupBy(KeyReader);
            foreach (var grouping in groupedItems)
            {
                var group = GetGroupOrCreate(grouping.Key);
                group.AddRange(grouping);
            }
        }
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
