// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    public class HashedCollection<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        #region Variables

        private ICollection<KeyValuePair<TKey, TValue>> _collect;
        private ConcurrentDictionary<TKey, TValue> _dict;

        #endregion

        #region Constructors

        public HashedCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            _collect = collection;
            _dict = new ConcurrentDictionary<TKey, TValue>();
            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                _dict.TryAdd(item.Key, item.Value);
            }
        }

        #endregion

        #region Properties

        public int Count => _collect.Count;

        public bool IsReadOnly { get; }

        public TValue this[TKey key]
        {
            get => _dict.ContainsKey(key) ? _dict[key] : default;

            set => _dict[key] = value;
        }

        public ICollection<TKey> Keys => _dict.Keys;

        public ICollection<TValue> Values => _dict.Values;

        #endregion

        #region Methods

        public void Add(TKey key, TValue value)
        {
            _collect.Add(new KeyValuePair<TKey, TValue>(key, value));
            _dict.TryAdd(key, value);
        }

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

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Insert(int position, TKey key, TValue value)
        {
            Insert(position, new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Insert(int position, KeyValuePair<TKey, TValue> item)
        {
            if(!TryInsert(position, item))
            {
                throw new InvalidOperationException("Insert is not support with " + _collect.GetType());
            }
        }

        public void TryInsert(int position, TKey key, TValue value)
        {
            TryInsert(position, new KeyValuePair<TKey, TValue>(key, value));
        }

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

        public void Clear()
        {
            _collect.Clear();
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key) && _dict[item.Key].Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dict.Values.Any(v => v.Equals(value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        public bool Remove(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value) && Remove(new KeyValuePair<TKey, TValue>(key, _dict[key]));
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _collect.Remove(item);
            return _dict.TryRemove(item.Key, out TValue _);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        #endregion
    }
}