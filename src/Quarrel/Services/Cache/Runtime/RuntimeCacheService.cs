// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Quarrel.ViewModels.Services.Cache.Runtime;

namespace Quarrel.Services.Cache.Runtime
{
    /// <summary>
    /// A simple <see langword="class"/> that handles a typed, runtime cache
    /// </summary>
    public sealed class RuntimeCacheService : IRuntimeCacheService
    {
        // The synchronization semaphore slim for the cache map
        private readonly SemaphoreSlim CacheSemaphoreSlim = new SemaphoreSlim(1, 1);

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
            await CacheSemaphoreSlim.WaitAsync();
            try
            {
                string _key = $"{scope}/{key}";
                if (CacheMap.TryGetValue(_key, out object value)) return value as T;
                return CacheMap.GetOrAdd(_key, await producer()) as T;
            }
            finally
            {
                CacheSemaphoreSlim.Release();
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
            await CacheSemaphoreSlim.WaitAsync();
            try
            {
                foreach (string key in CacheMap.Keys.Where(k => k.StartsWith($"{scope}/")).ToArray())
                    CacheMap.TryRemove(key, out _);
            }
            finally
            {
                CacheSemaphoreSlim.Release();
            }
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            await CacheSemaphoreSlim.WaitAsync();
            try
            {
                    CacheMap.Clear();
            }
            finally
            {
                CacheSemaphoreSlim.Release();
            }
        }
    }
}
