// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// An observable collection of items with hashed lookup.
    /// </summary>
    /// <typeparam name="TKey">The key type for lookup.</typeparam>
    /// <typeparam name="TValue">The type of items.</typeparam>
    public class ObservableHashedCollection<TKey, TValue> :
        ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>,
        IEnumerable<TValue>,
        INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly SynchronizationContext _context;
        private HashedCollection<TKey, TValue> _hashedCollection;
        private List<string> _propertiesToUpdate = new List<string> { "Count", "Keys", "Values" };

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashedCollection{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection">The initial items in the collection.</param>
        public ObservableHashedCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            _context = SynchronizationContext.Current;
            _hashedCollection = new HashedCollection<TKey, TValue>(collection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableHashedCollection{TKey, TValue}"/> class.
        /// </summary>
        public ObservableHashedCollection()
        {
            _context = SynchronizationContext.Current;
            _hashedCollection = new HashedCollection<TKey, TValue>(new List<KeyValuePair<TKey, TValue>>());
        }

        /// <summary>
        /// Fired when the contents change.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public int Count => _hashedCollection.Count;

        /// <inheritdoc/>
        public ICollection<TKey> Keys => _hashedCollection.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => _hashedCollection.Values;

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => _hashedCollection.IsReadOnly;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get => _hashedCollection[key];
            set => UpdateWithNotification(key, value);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            TryAddWithNotification(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            _hashedCollection.Clear();
            NotifyObserversOfChange();
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return _hashedCollection.Contains(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _hashedCollection.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return TryRemoveWithNotification(item.Key);
        }

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _hashedCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
        {
            return _hashedCollection.Values.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hashedCollection.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            TryAddWithNotification(key, value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _hashedCollection.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return TryRemoveWithNotification(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _hashedCollection.TryGetValue(key, out value);
        }

        /// <summary>
        /// Clears all items from the collection.
        /// </summary>
        public void Clear()
        {
            ClearWithNotification();
        }

        private void NotifyObserversOfChange()
        {
            var collectionHandler = CollectionChanged;
            var propertyHandler = PropertyChanged;
            if (collectionHandler != null || propertyHandler != null)
            {
                _context.Post(
                    s =>
                    {
                        collectionHandler?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        if (propertyHandler != null)
                        {
                            foreach (string property in _propertiesToUpdate)
                            {
                                propertyHandler(this, new PropertyChangedEventArgs(property));
                            }
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
            if (result)
            {
                NotifyObserversOfChange();
            }

            return result;
        }

        private bool TryRemoveWithNotification(TKey key)
        {
            bool result = _hashedCollection.Remove(key);
            if (result)
            {
                NotifyObserversOfChange();
            }

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
    }
}