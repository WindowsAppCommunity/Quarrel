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
    public class HashDeque<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        #region Variables

        private Deque<KeyValuePair<TKey, TValue>> _deque;
        private Dictionary<TKey, TValue> _dict;

        #endregion

        #region Constructors

        public HashDeque()
        {
            _deque = new Deque<KeyValuePair<TKey, TValue>>();
            _dict = new Dictionary<TKey, TValue>();
        }

        public HashDeque(IDictionary<TKey, TValue> dictionary)
        {
            _deque = new Deque<KeyValuePair<TKey, TValue>>(dictionary.AsEnumerable());
            _dict = new Dictionary<TKey, TValue>(dictionary);
        }

        #endregion

        #region Properties

        public int Count => _deque.Count;

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
            _deque.AddToFront(new KeyValuePair<TKey, TValue>(key, value));
            _dict.Add(key, value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            bool result = _dict.TryAdd(key, value);
            if (result) _deque.AddToFront(new KeyValuePair<TKey, TValue>(key, value));
            return result;
        }

        public void AddToBack(TKey key, TValue value)
        {
            _deque.AddToBack(new KeyValuePair<TKey, TValue>(key, value));
            _dict.Add(key, value);
        }

        public bool TryAddToBack(TKey key, TValue value)
        {
            bool result = _dict.TryAdd(key, value);
            if (result) _deque.AddToBack(new KeyValuePair<TKey, TValue>(key, value));
            return result;
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void AddToBack(KeyValuePair<TKey, TValue> item)
        {
            AddToBack(item.Key, item.Value);
        }

        public void Clear()
        {
            _deque.Clear();
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
            _deque.Remove(item);
            return _dict.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dict.TryGetValue(key, out value);
        }

        #endregion
    }
}