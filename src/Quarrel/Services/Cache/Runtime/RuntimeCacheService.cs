// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using Quarrel.ViewModels.Services.Cache.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quarrel.Services.Cache.Runtime
{
    /// <summary>
    /// A simple <see langword="class"/> that handles a typed, runtime cache.
    /// </summary>
    public sealed class RuntimeCacheService : IRuntimeCacheService
    {
        // The synchronization semaphore slim for the cache map
        private readonly SemaphoreSlim _cacheSemaphoreSlim = new SemaphoreSlim(1, 1);

        // The dictionary with the cached items
        private readonly ConcurrentDictionary<string, object> _cacheMap = new ConcurrentDictionary<string, object>();

        /// <inheritdoc/>
        public T TryGetValue<T>(string key, string scope = null)
            where T : class
        {
            return _cacheMap.TryGetValue($"{scope}/{key}", out object value) && value is T result
                ? result
                : default;
        }

        /// <inheritdoc/>
        public T TryGetValue<T>(string key, Func<T> producer, string scope = null)
            where T : class
        {
            return _cacheMap.GetOrAdd($"{scope}/{key}", _ => producer()) as T;
        }

        /// <inheritdoc/>
        public async Task<T> TryGetValueAsync<T>(string key, Func<Task<T>> producer, string scope = null)
            where T : class
        {
            await _cacheSemaphoreSlim.WaitAsync();
            try
            {
                key = $"{scope}/{key}";
                if (_cacheMap.TryGetValue(key, out object value))
                {
                    return value as T;
                }

                return _cacheMap.GetOrAdd(key, await producer()) as T;
            }
            finally
            {
                _cacheSemaphoreSlim.Release();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyList<T> TryGetValues<T>(string scope = null)
            where T : class
        {
            return (
                from pair in _cacheMap
                where pair.Key.StartsWith($"{scope}/") && pair.Value is T
                select pair.Value as T).ToArray();
        }

        /// <inheritdoc/>
        public void SetValue<T>(string key, T value, string scope = null)
            where T : class
        {
            _cacheMap.AddOrUpdate($"{scope}/{key}", value, (_, old) => value);
        }

        /// <inheritdoc/>
        public void Remove(string key, string scope = null)
        {
            _cacheMap.TryRemove($"{scope}/{key}", out _);
        }

        /// <inheritdoc/>
        public async Task ClearScopeAsync(string scope = null)
        {
            await _cacheSemaphoreSlim.WaitAsync();
            try
            {
                foreach (string key in _cacheMap.Keys.Where(k => k.StartsWith($"{scope}/")).ToArray())
                {
                    _cacheMap.TryRemove(key, out _);
                }
            }
            finally
            {
                _cacheSemaphoreSlim.Release();
            }
        }

        /// <inheritdoc/>
        public async Task ClearAsync()
        {
            await _cacheSemaphoreSlim.WaitAsync();
            try
            {
                _cacheMap.Clear();
            }
            finally
            {
                _cacheSemaphoreSlim.Release();
            }
        }
    }
}
