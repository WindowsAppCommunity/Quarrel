using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace System.Collections.ObjectModel
{
    public class SortedObservableCollection<TKey, TValue> :
        IEnumerable<TValue>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;
        private SortedList<TKey, TValue> _sortedList;

        public SortedObservableCollection(List<TValue> list, Func<TValue, TKey> func)
        {
            SortedList<TKey, TValue> temp = new SortedList<TKey, TValue>();
            foreach (var item in list)
            {
                temp.Add(func(item), item);
            }
            _sortedList = temp;
        }

        public SortedObservableCollection(SortedList<TKey, TValue> collection)
        {
            _context = SynchronizationContext.Current;
            _sortedList = new SortedList<TKey, TValue>(collection);
        }

        public SortedObservableCollection()
        {
            _context = SynchronizationContext.Current;
            _sortedList = new SortedList<TKey, TValue>(new SortedList<TKey, TValue>());
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected List<string> PropertiesToUpdate = new List<string>{"Count", "Keys", "Values"};

        protected virtual void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                _context.Post(s =>
                {
                    collectionHandler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    if (propertyHandler != null)
                    {
                        foreach (string property in PropertiesToUpdate)
                        {
                            propertyHandler(this, new PropertyChangedEventArgs(property));
                        }
                    }
                }, null);
            }
        }

        private void AddWithNotification(KeyValuePair<TKey, TValue> item)
        {
            AddWithNotification(item.Key, item.Value);
        }

        private void AddWithNotification(TKey key, TValue value)
        {
            _sortedList.Add(key, value);
            NotifyObserversOfChange();
        }

        private void RemoveWithNotification(TKey key)
        {
            _sortedList.Remove(key);
            NotifyObserversOfChange();
        }

        private void UpdateWithNotification(TKey key, TValue value)
        {
            _sortedList[key] = value;
            NotifyObserversOfChange();
        }

        private void ClearWithNotification()
        {
            _sortedList.Clear();
            NotifyObserversOfChange();
        }

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public int Count => _sortedList.Count;

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return _sortedList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sortedList.GetEnumerator();
        }

        #endregion

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            AddWithNotification(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _sortedList.ContainsKey(key);
        }

        public ICollection<TKey> Keys => _sortedList.Keys;

        public void Remove(TKey key)
        {
            RemoveWithNotification(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _sortedList.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => _sortedList.Values;

        public TValue this[TKey key]
        {
            get => _sortedList[key];
            set => UpdateWithNotification(key, value);
        }

        #endregion

        public void Clear()
        {
            ClearWithNotification();
        }
    }

}