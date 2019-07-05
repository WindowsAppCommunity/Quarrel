using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;

namespace System.Collections.ObjectModel
{
    public class ObservableHashDeque<TKey, TValue> :
         ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
         INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;
        private HashDeque<TKey, TValue> _hashDeque;

        public ObservableHashDeque()
        {
            _context = AsyncOperationManager.SynchronizationContext;
            _hashDeque = new HashDeque<TKey, TValue>();
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
            bool result = true;  _hashDeque.Add(key, value);
            if(result) NotifyObserversOfChange();
            return result;
        }

        private bool TryAddToBackWithNotification(KeyValuePair<TKey, TValue> item)
        {
            return TryAddToBackWithNotification(item.Key, item.Value);
        }

        private bool TryAddToBackWithNotification(TKey key, TValue value)
        {
            bool result = _hashDeque.TryAddToBack(key, value);
            if(result) NotifyObserversOfChange();
            return result;
        }

        private bool TryRemoveWithNotification(TKey key, out TValue value)
        {
            bool result = _hashDeque.Remove(key, out value);
            if (result) NotifyObserversOfChange();
            return result;
        }

        private void UpdateWithNotification(TKey key, TValue value)
        {
            _hashDeque[key] = value;
            NotifyObserversOfChange();
        }

        private void ClearWithNotification()
        {
            _hashDeque.Clear();
            NotifyObserversOfChange();
        }

        #region ICollection<KeyValuePair<TKey,TValue>> Members
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            TryAddWithNotification(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            _hashDeque.Clear();
            NotifyObserversOfChange();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _hashDeque.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _hashDeque.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count => _hashDeque.Count;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _hashDeque.IsReadOnly;

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue temp;
            return TryRemoveWithNotification(item.Key, out temp);
        }
        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _hashDeque.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hashDeque.GetEnumerator();
        }
        #endregion

        #region IDictionary<TKey,TValue> Members
        public void Add(TKey key, TValue value)
        {
            TryAddWithNotification(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return _hashDeque.ContainsKey(key);
        }

        public ICollection<TKey> Keys => _hashDeque.Keys;

        public bool Remove(TKey key)
        {
            TValue temp;
            return TryRemoveWithNotification(key, out temp);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _hashDeque.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values => _hashDeque.Values;

        public TValue this[TKey key]
        {
            get => _hashDeque[key];
            set => UpdateWithNotification(key, value);
        }

        #endregion

        public void AddToBack(TKey key, TValue value)
        {
            TryAddToBackWithNotification(key, value);
        }

        public void AddToBack(KeyValuePair<TKey, TValue> pair)
        {
            AddToBack(pair.Key, pair.Value);
        }

        public void Clear()
        {
            ClearWithNotification();
        }
    }

}