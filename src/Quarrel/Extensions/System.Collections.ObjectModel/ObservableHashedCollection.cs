using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace System.Collections.ObjectModel
{
    public class ObservableHashedCollection<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
        IEnumerable<TValue>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;
        private HashedCollection<TKey, TValue> _hashedCollection;

        public ObservableHashedCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _hashedCollection = new HashedCollection<TKey, TValue>(collection);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyObserversOfChange()
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
                        propertyHandler(this, new PropertyChangedEventArgs("Count"));
                        propertyHandler(this, new PropertyChangedEventArgs("Keys"));
                        propertyHandler(this, new PropertyChangedEventArgs("Values"));
                    }
                }, null);
            }
        }

        private bool TryAddWithNotification(KeyValuePair<TKey, TValue> item)
        {
            return TryAddWithNotification(item.Key, item.Value);
        }

        private bool TryAddWithNotification(TKey key, TValue value)
        {
            bool result = _hashedCollection.TryAdd(key, value);
            if (result) NotifyObserversOfChange();
            return result;
        }

        private bool TryRemoveWithNotification(TKey key, out TValue value)
        {
            bool result = _hashedCollection.Remove(key, out value);
            if (result) NotifyObserversOfChange();
            return result;
        }

        private void UpdateWithNotification(TKey key, TValue value)
        {
            _hashedCollection[key] = value;
            NotifyObserversOfChange();
        }

        private void ClearWithNotification()
        {
            _hashedCollection.Clear();
            NotifyObserversOfChange();
        }

        #region ICollection<KeyValuePair<TKey,TValue>> Members
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            TryAddWithNotification(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            _hashedCollection.Clear();
            NotifyObserversOfChange();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _hashedCollection.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _hashedCollection.CopyTo(array, arrayIndex);
        }

        public int Count => _hashedCollection.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _hashedCollection.IsReadOnly;

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue temp;
            return TryRemoveWithNotification(item.Key, out temp);
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _hashedCollection.GetEnumerator();
        }

        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return _hashedCollection.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hashedCollection.GetEnumerator();
        }
        #endregion

        #region IDictionary<TKey,TValue> Members
        public void Add(TKey key, TValue value)
        {
            TryAddWithNotification(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _hashedCollection.ContainsKey(key);
        }

        public ICollection<TKey> Keys => _hashedCollection.Keys;

        public bool Remove(TKey key)
        {
            TValue temp;
            return TryRemoveWithNotification(key, out temp);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _hashedCollection.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => _hashedCollection.Values;

        public TValue this[TKey key]
        {
            get => _hashedCollection[key];
            set => UpdateWithNotification(key, value);
        }

        #endregion

        public void Clear()
        {
            ClearWithNotification();
        }
    }

}