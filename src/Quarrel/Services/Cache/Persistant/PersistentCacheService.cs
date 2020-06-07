// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using JetBrains.Annotations;
using Newtonsoft.Json;
using Quarrel.ViewModels.Services.Cache.Persistent;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

namespace Quarrel.Services.Cache.Persistent
{
    /// <summary>
    /// A simple <see langword="class"/> that handles a typed, persistent cache.
    /// </summary>
    public sealed class PersistentCacheService : IPersistentCacheService
    {
        /// <summary>
        /// The default file extension for all cached files.
        /// </summary>
        private const string FileExtension = ".json";

        /// <summary>
        /// The synchronization semaphore slim for the remote cache APIs.
        /// </summary>
        [NotNull]
        private readonly SemaphoreSlim _cacheSemaphoreSlim = new SemaphoreSlim(1, 1);
        private HttpClient _httpClient;

        /// <inheritdoc/>
        public ICacheStorageProvider Roaming { get; } = new CacheStorageProvider(ApplicationData.Current.RoamingFolder);

        /// <inheritdoc/>
        public ICacheStorageProvider Local { get; } = new CacheStorageProvider(ApplicationData.Current.LocalCacheFolder);

        /// <summary>
        /// Gets the <see cref="Windows.Web.Http.HttpClient"/> instance to use to download PDF documents.
        /// </summary>
        [NotNull]
        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                    _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3710.0 Safari/537.36");
                }

                return _httpClient;
            }
        }

        /// <inheritdoc/>
        public async Task<object> GetRemoteFileAsync(string url)
        {
            string filename = $"{url.BernsteinHash()}";
            if (!(await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(filename) is StorageFile file))
            {
                using (IInputStream input = await HttpClient.GetInputStreamAsync(new Uri(url)))
                using (Stream stream = input.AsStreamForRead())
                {
                    await _cacheSemaphoreSlim.WaitAsync();
                    try
                    {
                        file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(
                            filename,
                            CreationCollisionOption.ReplaceExisting);
                        using (Stream output = await file.OpenStreamForWriteAsync())
                        {
                            await stream.CopyToAsync(output);
                        }
                    }
                    finally
                    {
                        _cacheSemaphoreSlim.Release();
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// A <see langword="class"/> that handles cached items on a specific storage location.
        /// </summary>
        private sealed class CacheStorageProvider : ICacheStorageProvider
        {
            /// <summary>
            /// The semaphore slim for the cache storage provider.
            /// </summary>
            [NotNull]
            private readonly SemaphoreSlim _cacheSemaphoreSlim = new SemaphoreSlim(1, 1);

            /// <summary>
            /// The dictionary with the cached items.
            /// </summary>
            [NotNull]
            private readonly ConcurrentDictionary<string, object> _cacheMap = new ConcurrentDictionary<string, object>();

            /// <summary>
            /// The target folder to use to store and retrieve the cached values.
            /// </summary>
            [NotNull]
            private readonly StorageFolder _cacheFolder;

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheStorageProvider"/> class that works on a specific <see cref="StorageFolder"/>.
            /// </summary>
            /// <param name="folder">The target <see cref="StorageFolder"/> instance to use to store the cached values.</param>
            public CacheStorageProvider([NotNull] StorageFolder folder) => _cacheFolder = folder;

            /// <inheritdoc/>
            public async Task<T> TryGetValueAsync<T>(string key, string scope = null)
                where T : class, new()
            {
                await _cacheSemaphoreSlim.WaitAsync();
                try
                {
                    // Try to get the value from the runtime cache
                    var (hex, filename) = GetCacheKeys(key, scope);
                    if (_cacheMap.TryGetValue(hex, out object value))
                    {
                        if (value is T result)
                        {
                            return result;
                        }
                    }

                    // Fallback to disk
                    if (await _cacheFolder.TryGetItemAsync(filename) is IStorageFile file)
                    {
                        string json = await FileIO.ReadTextAsync(file);
                        T result = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<T>(json);
                        _cacheMap.AddOrUpdate(hex, result); // Store in runtime cache for faster access in the future
                        return result;
                    }

                    return null;
                }
                finally
                {
                    _cacheSemaphoreSlim.Release();
                }
            }

            /// <inheritdoc/>
            public async Task<IReadOnlyList<T>> TryGetValuesAsync<T>(string scope)
                where T : class, new()
            {
                await _cacheSemaphoreSlim.WaitAsync();
                try
                {
                    IReadOnlyList<StorageFile> files = await _cacheFolder.GetFilesAsync();
                    return await Task.WhenAll(files.Where(file => file.DisplayName.StartsWith($"{scope}_")).Select(async file =>
                    {
                        string
                            hex = Regex.Match(file.DisplayName, $"{scope}_([0-9A-F]+)$").Groups[1].Value,
                            json = await FileIO.ReadTextAsync(file);
                        T result = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<T>(json);
                        _cacheMap.AddOrUpdate(hex, result); // Store in runtime cache for faster access in the future
                        return result;
                    }));
                }
                finally
                {
                    _cacheSemaphoreSlim.Release();
                }
            }

            /// <inheritdoc/>
            public async Task SetValueAsync<T>(string key, T value, string scope = null)
                where T : class, new()
            {
                await _cacheSemaphoreSlim.WaitAsync();
                try
                {
                    var (hex, filename) = GetCacheKeys(key, scope);
                    StorageFile file = await _cacheFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(value));
                    _cacheMap.AddOrUpdate(hex, value);
                }
                finally
                {
                    _cacheSemaphoreSlim.Release();
                }
            }

            /// <inheritdoc/>
            public async Task DeleteValueAsync(string key, string scope = null)
            {
                await _cacheSemaphoreSlim.WaitAsync();
                try
                {
                    var (hex, filename) = GetCacheKeys(key, scope);
                    if (await _cacheFolder.TryGetItemAsync(filename) is StorageFile file)
                    {
                        await file.DeleteAsync();
                        _cacheMap.TryRemove(hex, out var _);
                    }
                }
                finally
                {
                    _cacheSemaphoreSlim.Release();
                }
            }

            /// <inheritdoc/>
            public async Task DeleteValuesAsync(string scope = null)
            {
                await _cacheSemaphoreSlim.WaitAsync();
                try
                {
                    IReadOnlyList<StorageFile> files = await _cacheFolder.GetFilesAsync();
                    await Task.WhenAll(files.Where(file => file.DisplayName.StartsWith($"{scope}_")).Select(file =>
                    {
                        string hex = Regex.Match(file.DisplayName, $"{scope}_([0-9A-F]+)$").Groups[1].Value;
                        if (_cacheMap.ContainsKey(hex))
                        {
                            _cacheMap.TryRemove(hex, out var _);
                        }

                        return file.DeleteAsync().AsTask();
                    }));
                }
                finally
                {
                    _cacheSemaphoreSlim.Release();
                }
            }

            /// <summary>
            /// Gets the item key and filename.
            /// </summary>
            [Pure]
            private static (string Key, string Filename) GetCacheKeys([NotNull] string key, [CanBeNull] string scope)
            {
                string name = scope == null
                    ? key.ToHex()
                    : $"{scope}_{key.ToHex()}";
                return (name, $"{name}{FileExtension}");
            }
        }
    }
}
