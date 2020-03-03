// Copyright (c) Quarrel. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A Collection that notifies observers of change and allows scrolling past the loaded region.
    /// </summary>
    /// <typeparam name="T">Type of item in the list.</typeparam>
    /// <remarks>
    /// Partially implemented.</remarks>
    public class ObservableLazyLoadingCollection<T> : IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly IList<T> _list = new List<T>();

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc/>
        int ICollection<T>.Count => _list.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _list.IsReadOnly;

        /// <inheritdoc/>
        int IReadOnlyCollection<T>.Count => _list.Count;

        /// <inheritdoc/>
        public T this[int index]
        {
            get => _list[index];
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        /// <summary>
        /// Replaces a range of items in the collection.
        /// </summary>
        /// <param name="index">Starting index.</param>
        /// <param name="items">New items.</param>
        public void ReplaceRange(int index, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                _list[++index] = item;
            }
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
