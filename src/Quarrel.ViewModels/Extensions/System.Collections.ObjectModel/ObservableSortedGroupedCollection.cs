using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.ObjectModel
{

    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class. 
        /// </summary> 
        public ObservableRangeCollection()
            : base()
        {
        }

        /// <summary> 
        /// Initializes a new instance of the System.Collections.ObjectModel.ObservableCollection(Of T) class that contains elements copied from the specified collection. 
        /// </summary> 
        /// <param name="collection">collection: The collection from which the elements are copied.</param> 
        /// <exception cref="System.ArgumentNullException">The collection parameter cannot be null.</exception> 
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary> 
        /// Adds the elements of the specified collection to the end of the ObservableCollection(Of T). 
        /// </summary> 
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            InsertRange(Count, collection, notificationMode);
        }

        /// <summary> 
        /// Inserts the elements of the specified collection at the index specified of the ObservableCollection(Of T). 
        /// </summary> 
        public void InsertRange(int index, IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Add or Reset for AddRange.", "notificationMode");
            if (collection == null)
                throw new ArgumentNullException("collection");

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                ((List<T>) Items).InsertRange(index, collection);

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);

            // Don't send event if there's no data!
            if (changedItems.Count == 0) return;

            ((List<T>)Items).InsertRange(index, changedItems);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, index));
        }

        /// <summary> 
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
        /// </summary> 
        public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
        {
            if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
                throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", "notificationMode");
            if (collection == null)
                throw new ArgumentNullException("collection");

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {

                foreach (var i in collection)
                    Items.Remove(i);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            for (int i = 0; i < changedItems.Count; i++)
            {
                if (!Items.Remove(changedItems[i]))
                {
                    changedItems.RemoveAt(i); //Can't use a foreach because changedItems is intended to be (carefully) modified
                    i--;
                }
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, -1));
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified item. 
        /// </summary> 
        public void Replace(T item)
        {
            ReplaceRange(new T[] { item });
        }

        /// <summary> 
        /// Clears the current collection and replaces it with the specified collection. 
        /// </summary> 
        public void ReplaceRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            Items.Clear();
            AddRange(collection, NotifyCollectionChangedAction.Reset);
        }

    }


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
