using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NamedPipeWrapper
{
    internal static class CollectionExtensions
    {
        public static void CopyTo(this Array source, Array destination)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (destination.Length < source.Length) throw new InvalidOperationException("Target array size must be greater or equal to source array size!");

            Array.Copy(source, 0, destination, 0, source.Length);
        }

        public static T[] CloneArray<T>(this T[] source) where T : struct
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var result = new T[source.Length];
            Array.Copy(source, 0, result, 0, source.Length);

            return result;
        }

        public static bool HashsetEquals<T>(this HashSet<T> source, HashSet<T> compareTo)
        {
            if (source == null && compareTo == null)
            {
                return true;
            }

            if (ReferenceEquals(source, compareTo))
            {
                return true;
            }

            if (source == null || compareTo == null)
            {
                return false;
            }

            return source.SetEquals(compareTo);
        }

        public static SortedDictionary<TKey, TValue> Clone<TKey, TValue>(this SortedDictionary<TKey, TValue> source)
        {
            return new SortedDictionary<TKey, TValue>(source, source.Comparer);
        }

        public static SortedSet<T> Clone<T>(this SortedSet<T> source)
        {
            return new SortedSet<T>(source, source.Comparer);
        }

        public static SortedList<TKey, TValue> Clone<TKey, TValue>(this SortedList<TKey, TValue> source)
        {
            return new SortedList<TKey, TValue>(source, source.Comparer);
        }

        public static bool Contains<T>(this ConcurrentBag<T> bag, T item)
        {
            T result;
            return bag.TryPeek(out result);
        }

        public static bool Remove<T>(this ConcurrentBag<T> bag, T item)
        {
            T dummy;
            return bag.TryTake(out dummy);
        }

	    public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T element)
	    {
		    if (source == null) throw new ArgumentNullException("source");

		    return source.Where(item => !Equals(item, element));
	    }

	    public static IReadOnlyList<T> CastToList<T>(this object source)
	    {
		    return (IReadOnlyList<T>)source;
	    }

	    public static int GetArrayHashCode<T>(this T[] array)
	    {
		    return ((IStructuralEquatable)array).GetHashCode(EqualityComparer<object>.Default);
	    }

        public static bool EqualsTo(this byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> source)
        {
            foreach (var pair in source)
            {
                dictionary[pair.Key] = pair.Value;
            }
        }

		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IEnumerable<TValue> source, Func<TValue, TKey> keyFunc)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");
			if (source == null) throw new ArgumentNullException("source");
			if (keyFunc == null) throw new ArgumentNullException("keyFunc");

			foreach (var item in source)
			{
				dictionary[keyFunc(item)] = item;
			}
		}

	    public static bool EqualsTo<TSrc, TDst>(this IReadOnlyList<TSrc> source, List<TDst> destination, Func<TSrc, TDst, bool> comparer)
        {
            if (ReferenceEquals(source, destination))
                return true;

            if (source == null || destination == null)
                return false;

            if (source.Count != destination.Count)
                return false;

            for (int i = 0; i < source.Count; i++)
            {
                if (!comparer(source[i], destination[i]))
                    return false;
            }

            return true;
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> source)
        {
            foreach (var item in source)
                collection.Add(item);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static HashSet<T> ConcatHashSet<T>(this IEnumerable<T> source, IEnumerable<T> target)
        {
            var result = CreateHashSetFromSource(source);
            foreach (var item in target)
                result.Add(item);

            return result;
        }

        public static HashSet<T> ConcatHashSet<T>(this IEnumerable<T> source, params T[] set)
        {
            var result = CreateHashSetFromSource(source);
            foreach (var item in set)
                result.Add(item);

            return result;
        }

        private static HashSet<T> CreateHashSetFromSource<T>(IEnumerable<T> source)
        {
            var sourceSet = source as HashSet<T>;
            return sourceSet == null ? new HashSet<T>(source) : new HashSet<T>(sourceSet, sourceSet.Comparer);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        public static List<T> CreateEmptyIfNull<T>(this List<T> source)
        {
            return source ?? new List<T>();
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, T def)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                    return item;
            }

            return def;
        }

        public static T LastOrDefault<T>(this IEnumerable<T> source, Func<T, bool> predicate, T def)
        {
            var result = def;
            foreach (var item in source)
            {
                if (predicate(item))
                    result = item;
            }

            return result;
        }

        public static TValue Max<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, double> keySelector, Func<TSource, TValue> valueSelector)
        {
            double max = Double.MinValue;
            TValue value = default(TValue);

            foreach (var item in source)
            {
                var key = keySelector(item);
                if (key > max)
                {
                    max = key;
                    value = valueSelector(item);
                }
            }

            return value;
        }

        public static TValue Max<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector) where TKey : IComparable<TKey>
        {
            return SelectByComparison(source, keySelector, valueSelector, 1);
        }

        public static TValue Min<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector) where TKey : IComparable<TKey>
        {
            return SelectByComparison(source, keySelector, valueSelector, -1);
        }

        private static TValue SelectByComparison<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, 
            Func<TSource, TValue> valueSelector,
            int comparisonResult) where TKey : IComparable<TKey>
        {
            if (!source.Any())
            {
                return default(TValue);
            }

            var first = source.First();
            var curKey = keySelector(first);
            var curValue = valueSelector(first);

            foreach (var item in source.Skip(1))
            {
                var key = keySelector(item);
                if (key.CompareTo(curKey) == comparisonResult)
                {
                    curKey = key;
                    curValue = valueSelector(item);
                }
            }

            return curValue;
        }

        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            return dictionary.TryRemove(key, out value);
        }

        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary.AddOrUpdate(key, value, (key1, value1) => value);
        }

        public static IEnumerable<TElement> IterateQueueList<TElement, TContainer>(
            this TContainer root, 
            Func<TContainer, IEnumerable<TElement>> elementAccessor,
            Func<TContainer, IEnumerable<TContainer>> childrenAccessor)
        {
            if (root == null) throw new ArgumentNullException("root");
            if (elementAccessor == null) throw new ArgumentNullException("elementAccessor");
            if (childrenAccessor == null) throw new ArgumentNullException("childrenAccessor");

            var queue = new Queue<TContainer>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                foreach (var element in elementAccessor(item))
                {
                    yield return element;
                }

                foreach (var child in childrenAccessor(item))
                {
                    queue.Enqueue(child);
                }
            }
        }

        public static IEnumerable<TElement> IterateQueue<TElement, TContainer>(
            this TContainer root,
            Func<TContainer, TElement> elementAccessor,
            Func<TContainer, IEnumerable<TContainer>> childrenAccessor)
        {
            if (root == null) throw new ArgumentNullException("root");
            if (elementAccessor == null) throw new ArgumentNullException("elementAccessor");
            if (childrenAccessor == null) throw new ArgumentNullException("childrenAccessor");

            var queue = new Queue<TContainer>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                
                yield return elementAccessor(item);

                foreach (var child in childrenAccessor(item))
                {
                    queue.Enqueue(child);
                }
            }
        }

        public static IEnumerable<TElement> IterateQueue<TElement, TContainer>(
            this TContainer root,
            Func<TContainer, TElement> elementAccessor,
            Func<TContainer, TContainer> childAccessor) where TContainer : class 
        {
            if (root == null) throw new ArgumentNullException("root");
            if (elementAccessor == null) throw new ArgumentNullException("elementAccessor");
            if (childAccessor == null) throw new ArgumentNullException("childAccessor");

            var queue = new Queue<TContainer>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                yield return elementAccessor(item);

                var child = childAccessor(item);
                if (child != null)
                {
                    queue.Enqueue(child);
                }
            }
        }

	    public static SortedDictionary<TTargetKey, List<TValue>> ToSortedDictionaryOfLists<TKey, TTargetKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groups, Func<TKey, TTargetKey> keyFunc)
        {
            var result = new SortedDictionary<TTargetKey, List<TValue>>();

            foreach (var batch in groups.OrderBy(x => x.Key))
            {
                result.Add(keyFunc(batch.Key), new List<TValue>(batch));
            }

            return result;
        }

	    public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keyFunc, Func<TSource, TValue> valueFunc)
	    {
		    if (source == null) throw new ArgumentNullException(nameof(source));
		    if (keyFunc == null) throw new ArgumentNullException(nameof(keyFunc));
		    if (valueFunc == null) throw new ArgumentNullException(nameof(valueFunc));
		    
			var result = new ConcurrentDictionary<TKey, TValue>();
		    foreach (var item in source)
		    {
			    result.AddOrUpdate(keyFunc(item), valueFunc(item));
		    }

		    return result;
	    }
	}
}