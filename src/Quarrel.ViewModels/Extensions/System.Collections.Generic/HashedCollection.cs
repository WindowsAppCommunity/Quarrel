// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A collection with hashed lookup.
    /// </summary>
    /// <typeparam name="TKey">The key for lookups.</typeparam>
    /// <typeparam name="TValue">The values in the collection.</typeparam>
    public class HashedCollection<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
    {
        private ICollection<KeyValuePair<TKey, TValue>> _collect;
        private ConcurrentDictionary<TKey, TValue> _dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedCollection{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="collection">The collection of items to base off of.</param>
        public HashedCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            _collect = collection;
            _dict = new ConcurrentDictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                _dict.TryAdd(item.Key, item.Value);
            }
        }

        /// <inheritdoc/>
        public int Count => _collect.Count;

        /// <inheritdoc/>
        public bool IsReadOnly { get; }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => _dict.Keys;

        /// <inheritdoc/>
        public ICollection<TValue> Values => _dict.Values;

        /// <summary>
        /// Gets an item by key from the collection.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The value under <paramref name="key"/>.</returns>
        public TValue this[TKey key]
        {
            get => _dict.ContainsKey(key) ? _dict[key] : default;

            set => _dict[key] = value;
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            _collect.Add(new KeyValuePair<TKey, TValue>(key, value));
            _dict.TryAdd(key, value);
        }

        /// <summary>
        /// Will add an item if it does not conflict.
        /// </summary>
        /// <param name="key">Key of the item to add.</param>
        /// <param name="value">Value of the item to add.</param>
        /// <returns>Whether or not the item was added.</returns>
        public bool TryAdd(TKey key, TValue value)
        {
            try
            {
                _dict.TryAdd(key, value);
            }
            catch
            {
                return false;
            }

            _collect.Add(new KeyValuePair<TKey, TValue>(key, value));
            return true;
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Inserts an item at position.
        /// </summary>
        /// <param name="position">The position to insert the item.</param>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The value for the item.</param>
        public void Insert(int position, TKey key, TValue value)
        {
            Insert(position, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Inserts an item at position.
        /// </summary>
        /// <param name="position">The position to insert the item.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int position, KeyValuePair<TKey, TValue> item)
        {
            if (!TryInsert(position, item))
            {
                throw new InvalidOperationException("Insert is not support with " + _collect.GetType());
            }
        }

        /// <summary>
        /// Will insert an item if it does not conflict.
        /// </summary>
        /// <param name="position">Position to insert at.</param>
        /// <param name="key">Key of the item to insert.</param>
        /// <param name="value">The value of the item to insert.</param>
        /// <returns>Whether or not the item was added.</returns>
        public bool TryInsert(int position, TKey key, TValue value)
        {
            return TryInsert(position, new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// Will insert an item if it does not conflict.
        /// </summary>
        /// <param name="position">Position to insert at.</param>
        /// <param name="item">Item to insert.</param>
        /// <returns>Whether or not the item was added.</returns>
        public bool TryInsert(int position, KeyValuePair<TKey, TValue> item)
        {
            if (_collect is IList<KeyValuePair<TKey, TValue>> list)
            {
                _dict.TryAdd(item.Key, item.Value);
                list.Insert(position, item);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _collect.Clear();
            _dict.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key) && _dict[item.Key].Equals(item.Value);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="array">New array.</param>
        /// <param name="index">Starting index in new array.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>)_dict).Remove(key);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _collect.Remove(item);
            return _dict.TryRemove(item.Key, out TValue _);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }
    }
}