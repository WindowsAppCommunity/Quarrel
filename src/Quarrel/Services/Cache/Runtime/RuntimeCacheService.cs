// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quarrel.Services.Cache.Runtime
{
    /// <summary>
    /// A simple <see langword="class"/> that handles a typed, runtime cache
    /// </summary>
    public sealed class RuntimeCacheService : IRuntimeCacheService
    {
        // The synchronization mutex for the cache map
        private readonly AsyncMutex CacheMutex = new AsyncMutex();

        // The dictionary with the cached items
        private readonly ConcurrentDictionary<string, object> CacheMap = new ConcurrentDictionary<string, object>();

        /// <inheritdoc/>
        public T TryGetValue<T>(string key, string scope = null) where T : class
        {
            return CacheMap.TryGetValue($"{scope}/{key}", out object value) && value is T result
                ? result
                : default;
        }

        /// <inheritdoc/>
        public T TryGetValue<T>(string key, Func<T> producer, string scope = null) where T : class
        {
            return CacheMap.GetOrAdd($"{scope}/{key}", _ => producer()) as T;
        }

        /// <inheritdoc/>
        public async Task<T> TryGetValueAsync<T>(string key, Func<Task<T>> producer, string scope = null) where T : class
        {
            using (await CacheMutex.LockAsync())
            {
                string _key = $"{scope}/{key}";
                if (CacheMap.TryGetValue(_key, out object value)) return value as T;
                return CacheMap.GetOrAdd(_key, await producer()) as T;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<T> TryGetValues<T>(string scope = null) where T : class
        {
            return (
                from pair in CacheMap
                where pair.Key.StartsWith($"{scope}/") && pair.Value is T
                select pair.Value as T).ToArray();
        }

        /// <inheritdoc/>
        public void SetValue<T>(string key, T value, string scope = null) where T : class
        {
            CacheMap.AddOrUpdate($"{scope}/{key}", value, (_, old) => value);
        }

        /// <inheritdoc/>
        public void Remove(string key, string scope = null)
        {
            CacheMap.TryRemove($"{scope}/{key}", out _);
        }

        /// <inheritdoc/>
        public async Task ClearScopeAsync(string scope = null)
        {
            using (await CacheMutex.LockAsync())
                foreach (string key in CacheMap.Keys.Where(k => k.StartsWith($"{scope}/")).ToArray())
                    CacheMap.Remove(key, out _);
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            using (await CacheMutex.LockAsync())
                CacheMap.Clear();
        }
    }
}
