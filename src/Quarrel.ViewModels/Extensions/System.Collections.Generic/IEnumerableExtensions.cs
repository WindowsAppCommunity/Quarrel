// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace System.Collections.Generic
{
    /// <summary>
    /// An extension <see langword="class"/> for <see cref="IEnumerable{T}"/> types.
    /// </summary>
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// A fast implementation of the <see cref="Enumerable.Last{T}(IEnumerable{T})"/> LINQ method that works on <see cref="IList{T}"/> objects.
        /// </summary>
        /// <typeparam name="T">The type of items in the input <see cref="IList{T}"/>.</typeparam>
        /// <param name="items">The input <see cref="IList{T}"/> instance.</param>
        /// <returns>The last item in <paramref name="items"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>([NotNull] this IList<T> items) => items[items.Count - 1];

        /// <summary>
        /// Finds the index of the first item matching the input predicate.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The source collection.</param>
        /// <param name="predicate">The predicate to use to match items.</param>
        /// <returns>The first index where <paramref name="predicate"/> is true.</returns>
        [Pure]
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, [NotNull] Func<T, bool> predicate)
        {
            int index = 0;
            foreach (T item in items)
            {
                if (predicate(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the first item matching the input item.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="items">The source collection.</param>
        /// <param name="target">The item to use for the equality comparisons.</param>
        /// <param name="comparer">The comparer function to check items for equality.</param>
        /// <returns>The first index of <paramref name="target"/>.</returns>
        [Pure]
        public static int IndexOf<T>([NotNull] this IEnumerable<T> items, T target, [NotNull] Func<T, T, bool> comparer)
        {
            int index = 0;
            foreach (T item in items)
            {
                if (comparer(item, target))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Sorts a target list using the key returned by the given function.
        /// </summary>
        /// <typeparam name="TValue">The Type of each list item.</typeparam>
        /// <param name="list">The list to sort.</param>
        /// <param name="selector">A function that returns a comparable object from each item in the input list.</param>
        public static void Sort<TValue>([NotNull] this List<TValue> list, [NotNull] Func<TValue, IComparable> selector)
        {
            // Get the custom comparator and sort the list
            IComparer<TValue> comparer = Comparer<TValue>.Create((x1, x2) =>
            {
                IComparable c1 = selector(x1), c2 = selector(x2);
                if (c1 != null)
                {
                    return c1.CompareTo(c2);
                }

                return c2 == null ? 0 : -1;
            });
            list.Sort(comparer);
        }

        /// <summary>
        /// Tries to get the value associated with the specified key.
        /// </summary>
        /// <typeparam name="TKey">The key type for the input <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The value type for the input <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</typeparam>
        /// <param name="dictionary">The source <see cref="IReadOnlyDictionary{TKey,TValue}"/> instance.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value by a key or the default value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Pure]
        public static TValue TryGetValue<TKey, TValue>([NotNull] this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            return dictionary.TryGetValue(key, out TValue value) ? value : default;
        }

        /// <summary>
        /// Adds or updates a given key-value pair in the input dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type for the input <see cref="IDictionary{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The value type for the input <see cref="IDictionary{TKey,TValue}"/>.</typeparam>
        /// <param name="dictionary">The source <see cref="IDictionary{TKey,TValue}"/> instance.</param>
        /// <param name="key">The key of the value to add or update.</param>
        /// <param name="value">The value to add or update.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddOrUpdate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Creates a readonly mapping of the input values, using the provided key selector function.
        /// </summary>
        /// <typeparam name="TKey">The type of keys to use in the returned mapping.</typeparam>
        /// <typeparam name="TValue">The type of values to map.</typeparam>
        /// <param name="source">The input sequence of values to map.</param>
        /// <param name="keySelector">The key selector function to use to map the input values.</param>
        /// <returns>A readonly Dictionary of keys to items that match a key.</returns>
        [Pure]
        [NotNull]
        public static IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> ToReadOnlyLookup<TKey, TValue>(
            [NotNull] this IEnumerable<TValue> source,
            [NotNull] Func<TValue, TKey> keySelector)
        {
            ILookup<TKey, TValue> map = source.ToLookup(keySelector);
            return map.ToDictionary<IGrouping<TKey, TValue>, TKey, IReadOnlyList<TValue>>(group => group.Key, group => group.ToArray());
        }
    }
}
