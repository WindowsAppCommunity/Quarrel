using System;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using Nito.Collections;

namespace System.Collections.ObjectModel
{
    public class HashedCollection<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        #region Variables

        private ICollection<KeyValuePair<TKey, TValue>> _collect;
        private Dictionary<TKey, TValue> _dict;

        #endregion

        #region Constructors

        public HashedCollection(ICollection<KeyValuePair<TKey, TValue>> collection)
        {
            _collect = collection;
            _dict = new Dictionary<TKey, TValue>(collection.AsEnumerable());
        }

        #endregion

        #region Properties

        public int Count => _collect.Count;

        public bool IsReadOnly { get; }

        public TValue this[TKey key]
        {
            get => _dict[key];

            set => _dict[key] = value;
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => new Dictionary<TKey, TValue>.KeyCollection(_dict);

        public Dictionary<TKey, TValue>.ValueCollection Values => new Dictionary<TKey, TValue>.ValueCollection(_dict);

        #endregion

        #region Methods

        public void Add(TKey key, TValue value)
        {
            _collect.Add(new KeyValuePair<TKey, TValue>(key, value));
            _dict.Add(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            bool result = _dict.TryAdd(key, value);
            if (result) _collect.Add(new KeyValuePair<TKey, TValue>(key, value));
            return result;
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
                _dict.Add(item.Key, item.Value);
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
            return _dict.ContainsValue(value);
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
            return _dict.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        #endregion
    }
}