using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace System.Collections.ObjectModel
{
    public class FilteredSortedGroupedObservableHashedCollection<TKey, TGroup, TElement>
        : SortedGroupedObservableHashedCollection<TKey, TGroup, TElement>
        where TGroup : IEquatable<TGroup>, IComparable<TGroup>
        where TElement : class, IEquatable<TElement>, IComparable<TElement>
    {

        private readonly Func<HashedGrouping<TKey, TGroup, TElement>, bool> Filter;

        public FilteredSortedGroupedObservableHashedCollection([NotNull] Func<TElement, TGroup> keyReader, [NotNull] Func<HashedGrouping<TKey, TGroup, TElement>, int> sorter, [NotNull] Func<HashedGrouping<TKey, TGroup, TElement>, bool> filter, ICollection<KeyValuePair<TKey, HashedGrouping<TKey, TGroup, TElement>>> collection) : base(keyReader, sorter, collection)
        {
            PropertiesToUpdate.Add(nameof(FilteredValues));
            PropertiesToUpdate.Add(nameof(FilteredSortedValues));
            Filter = filter;
        }

        [NotNull]

        public ICollection<HashedGrouping<TKey, TGroup, TElement>> FilteredValues => Values.Distinct().Where(Filter).ToList();
        public ICollection<HashedGrouping<TKey, TGroup, TElement>> FilteredSortedValues => ValuesSorted.Distinct().Where(Filter).ToList();
    }
}
