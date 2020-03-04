// Copyright (c) Quarrel. All rights reserved.

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// ObservableCollection with range based methods.
    /// </summary>
    /// <typeparam name="T">Type of item in the list.</typeparam>
    public class ObservableRangeCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class.
        /// </summary>
        public ObservableRangeCollection()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableRangeCollection{T}"/> class that contains elements copied from the specified collection.
        /// </summary>
        /// <param name="collection">collection: The collection from which the elements are copied.</param>
        /// <exception cref="ArgumentNullException">The collection parameter cannot be null.</exception>
        public ObservableRangeCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">Collection of items to add.</param>
        /// <param name="notificationMode">What type of update to notify of.</param>
        public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            InsertRange(Count, collection, notificationMode);
        }

        /// <summary>
        /// Inserts the elements of the specified collection at the index specified of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="index">Index to insert first item in <paramref name="collection"/> at.</param>
        /// <param name="collection">Collection of items to insert.</param>
        /// <param name="notificationMode">What type of update to notify of.</param>
        public void InsertRange(int index, IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
        {
            if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException("Mode must be either Add or Reset for InsertRange.", "notificationMode");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                ((List<T>)Items).InsertRange(index, collection);

                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            ((List<T>)Items).InsertRange(index, changedItems);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            if (changedItems.Count > 0)
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changedItems, index));
            }
        }

        /// <summary>
        /// Removes the first occurence of each item in the specified collection from ObservableCollection(Of T). NOTE: with notificationMode = Remove, removed items starting index is not set because items are not guaranteed to be consecutive.
        /// </summary>
        /// <param name="collection">Items to remove.</param>
        /// <param name="notificationMode">Notification type to send.</param>
        public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Reset)
        {
            if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", "notificationMode");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            CheckReentrancy();

            if (notificationMode == NotifyCollectionChangedAction.Reset)
            {
                foreach (var i in collection)
                {
                    Items.Remove(i);
                }

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                return;
            }

            var changedItems = collection is List<T> ? (List<T>)collection : new List<T>(collection);
            for (int i = 0; i < changedItems.Count; i++)
            {
                if (!Items.Remove(changedItems[i]))
                {
                    // Can't use a foreach because changedItems is intended to be (carefully) modified
                    changedItems.RemoveAt(i);
                    i--;
                }
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, changedItems, -1));
        }

        /// <summary>
        /// Clears the current collection and replaces it with the specified collection.
        /// </summary>
        /// <param name="collection">New collection contents.</param>
        public void ReplaceCollection(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            Items.Clear();
            AddRange(collection, NotifyCollectionChangedAction.Reset);
        }
    }
}
