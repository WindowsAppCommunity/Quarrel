// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Services.Cache.Runtime
{
    /// <summary>
    /// The default <see langword="interface"/> for the runtime cache manager used in the app.
    /// </summary>
    public interface IRuntimeCacheService
    {
        /// <summary>
        /// Tries to retrieve a cached item with the given key and scope.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="key">The key of the item to look for.</param>
        /// <param name="scope">The optional scope of the item to look for.</param>
        /// <returns>The value for the key.</returns>
        [MustUseReturnValue]
        T TryGetValue<T>([CanBeNull] string key, [CanBeNull] string scope = null)
            where T : class;

        /// <summary>
        /// Tries to retrieve a cached item with the given key and scope.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="key">The key of the item to look for.</param>
        /// <param name="producer">A <see cref="Func{TResult}"/> to create the new instance to cache, when needed.</param>
        /// <param name="scope">The optional scope of the item to look for.</param>
        /// <returns>The value for the key.</returns>
        [MustUseReturnValue]
        T TryGetValue<T>([NotNull] string key, [NotNull] Func<T> producer, [CanBeNull] string scope = null)
            where T : class;

        /// <summary>
        /// Tries to retrieve a cached item with the given key and optional scope.
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve.</typeparam>
        /// <param name="key">The key of the item to look for.</param>
        /// <param name="producer">A <see cref="Func{TResult}"/> to create the new instance to cache, when needed.</param>
        /// <param name="scope">The optional scope of the item to look for.</param>
        /// <returns>The value for the key.</returns>
        [MustUseReturnValue]
        Task<T> TryGetValueAsync<T>([NotNull] string key, [NotNull] Func<Task<T>> producer, [CanBeNull] string scope = null)
            where T : class;

        /// <summary>
        /// Gets the collection of all cached items of a given type, in a specific cache scope.
        /// </summary>
        /// <typeparam name="T">The type of items to look for.</typeparam>
        /// <param name="scope">The optional scope of the items to retrieve (the default scope will be used, if <see langword="null"/>).</param>
        /// <returns>The collection of values for the keys.</returns>
        [Pure]
        [NotNull]
        [ItemNotNull]
        IReadOnlyList<T> TryGetValues<T>([CanBeNull] string scope = null)
            where T : class;

        /// <summary>
        /// Stores an item for later use.
        /// </summary>
        /// <typeparam name="T">The type of item to store.</typeparam>
        /// <param name="key">The key of the item to store.</param>
        /// <param name="value">The item to store.</param>
        /// <param name="scope">The optional scope of the item to store.</param>
        void SetValue<T>([NotNull] string key, [NotNull] T value, [CanBeNull] string scope = null)
            where T : class;

        /// <summary>
        /// Removes a cached value with the specified key.
        /// </summary>
        /// <param name="key">The key of the item to remove from the cache.</param>
        /// <param name="scope">The optional scope of the item to remove.</param>
        void Remove([NotNull] string key, [CanBeNull] string scope = null);

        /// <summary>
        /// Clears the items in the cache under a given scope.
        /// </summary>
        /// <param name="scope">The optional scope of the items to remove from the cache (the default scope will be used, if <see langword="null"/>).</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ClearScopeAsync([CanBeNull] string scope = null);

        /// <summary>
        /// Clears all the currently cached values.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ClearAsync();
    }
}