// Special thanks to Sergio Pedri for the basis of this design

using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using JetBrains.Annotations;

namespace Quarrel.Services.Cache.Persistent
{
    /// <summary>
    /// The default <see langword="interface"/> for the persistent cache manager used in the app
    /// </summary>
    public interface IPersistentCacheService
    {
        /// <summary>
        /// Gets the <see cref="ICacheStorageProvider"/> instance that works on the roaming folder
        /// </summary>
        [NotNull]
        ICacheStorageProvider Roaming { get; }

        /// <summary>
        /// Gets the <see cref="ICacheStorageProvider"/> instance that works on the local folder
        /// </summary>
        [NotNull]
        ICacheStorageProvider Local { get; }

        /// <summary>
        /// Gets a file with the contents of the remote URL
        /// </summary>
        /// <param name="url">The remote URL to download and cache</param>
        [ItemNotNull]
        Task<StorageFile> GetRemoteFileAsync([NotNull] string url);
    }

    /// <summary>
    /// The default <see langword="interface"/> for a cache storage provider
    /// </summary>
    public interface ICacheStorageProvider
    {
        /// <summary>
        /// Tries to retrieve a stored value with the given key
        /// </summary>
        /// <typeparam name="T">The type of item to retrieve</typeparam>
        /// <param name="key">The key of the item to retrieve</param>
        /// <param name="scope">The optional scope for the cached value</param>
        [ItemCanBeNull]
        Task<T> TryGetValueAsync<T>([NotNull] string key, [CanBeNull] string scope = null) where T : class, new();

        /// <summary>
        /// Tries to get all the values from a specific scope
        /// </summary>
        /// <typeparam name="T">The type of items in the scope</typeparam>
        /// <param name="scope">The target scope</param>
        [ItemNotNull]
        Task<IReadOnlyList<T>> TryGetValuesAsync<T>([NotNull] string scope) where T : class, new();

        /// <summary>
        /// Stores a new item in the cache
        /// </summary>
        /// <typeparam name="T">The type of item to store</typeparam>
        /// <param name="key">The key of the item to store</param>
        /// <param name="value">The item to store</param>
        /// <param name="scope">The optional scope for the cached value</param>
        Task SetValueAsync<T>([NotNull] string key, [NotNull] T value, [CanBeNull] string scope = null) where T : class, new();

        /// <summary>
        /// Deletes a cached value with the specified key, if existing
        /// </summary>
        /// <param name="key">The key of the cached item to delete</param>
        /// <param name="scope">The optional scope for the cached value</param>
        Task DeleteValueAsync([NotNull] string key, [CanBeNull] string scope = null);

        /// <summary>
        /// Deletes the cached values from a specific scope
        /// </summary>
        /// <param name="scope">The optional scope for the cached values</param>
        Task DeleteValuesAsync([CanBeNull] string scope = null);
    }
}