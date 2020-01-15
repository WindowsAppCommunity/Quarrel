using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace Quarrel.ViewModels.Extensions.System.Collections.Generic
{
    public class ObservableLazyLoadingCollection<T>: IList<T>, ICollection<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IEnumerable<T>, INotifyCollectionChanged
    {
        private readonly IList<T> _list = new List<T>();

        int ICollection<T>.Count => _list.Count;
        public bool IsReadOnly => _list.IsReadOnly;
        int IReadOnlyCollection<T>.Count => _list.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
        public bool Contains(T item)
        {
            return _list.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }


        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void ReplaceRange(int index, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                _list[++index] = item;
            }
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get => _list[index];
            set => throw new NotImplementedException();
        }

        public void UpdateTrueSize(int size)
        {
            int listCount = _list.Count;

            if (listCount < size)
            {
                for (int i = 0; i < size - listCount; i++)
                {
                    Add(default);
                }
            }
            else if(listCount > size)
            {
                for (int i = 0; i < listCount - size; i++)
                {
                    RemoveAt(_list.Count);
                }
            }
        }
    }
}
