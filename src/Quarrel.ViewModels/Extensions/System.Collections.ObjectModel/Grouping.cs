// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using Quarrel.ViewModels.Extensions.System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.ObjectModel
{
    /// <summary>
    /// A custom <see cref="ObservableCollection{T}"/> that implements the <see cref="IGrouping{TKey,TElement}"/> <see langword="interface"/>.
    /// </summary>
    /// <typeparam name="TKey">They type of the key of each group.</typeparam>
    /// <typeparam name="TElement">The type of elements in each group.</typeparam>
    public class Grouping<TKey, TElement> : ObservableCollection<TElement>, IGrouping<TKey, TElement>, IGrouping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping{TKey, TElement}"/> class.
        /// </summary>
        /// <param name="key">They key to use in the new instance.</param>
        public Grouping(TKey key) => Key = key;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grouping{TKey, TElement}"/> class instance with the specified key and items collection.
        /// </summary>
        /// <param name="key">The key to use in the new instance.</param>
        /// <param name="items">The initial collection of items to insert in the collection.</param>
        public Grouping(TKey key, [NotNull, ItemNotNull] IEnumerable<TElement> items) : this(key)
        {
            foreach (TElement item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Gets the key associated with the current group.
        /// </summary>
        public TKey Key { get; }

        /// <inheritdoc/>
        object IGrouping.Key => Key;
    }
}
