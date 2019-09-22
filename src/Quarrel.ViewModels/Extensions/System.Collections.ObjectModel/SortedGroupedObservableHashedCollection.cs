using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace System.Collections.ObjectModel
{
    public class SortedGroupedObservableHashedCollection<TKey, TGroup, TElement>
        : GroupedObservableHashedCollection<TKey, TGroup, TElement>
        where TGroup : IEquatable<TGroup>, IComparable<TGroup>
        where TElement : class, IEquatable<TElement>, IComparable<TElement>
    {

        private readonly Func<HashedGrouping<TKey, TGroup, TElement>, int> Sorter;


        public SortedGroupedObservableHashedCollection([NotNull] Func<TElement, TGroup> keyReader, [NotNull] Func<HashedGrouping<TKey, TGroup, TElement>, int> sorter, ICollection<KeyValuePair<TKey, HashedGrouping<TKey, TGroup, TElement>>> collection) : base(keyReader, collection)
        {
            PropertiesToUpdate.Add(nameof(ValuesSorted));
            Sorter = sorter;
        }

        [NotNull]

        public ICollection<HashedGrouping<TKey, TGroup, TElement>> ValuesSorted => Values.Distinct().OrderBy(Sorter).ToList();
    }
}
