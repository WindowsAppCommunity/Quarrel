// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Quarrel.Services.Cache.Persistent
{
    /// <summary>
    /// A simple <see langword="class"/> that handles a typed, persistent cache
    /// </summary>
    public sealed class PersistentCacheService : IPersistentCacheService
    {
        /// <inheritdoc/>
        public ICacheStorageProvider Roaming { get; } = new CacheStorageProvider(ApplicationData.Current.RoamingFolder);

        /// <inheritdoc/>
        public ICacheStorageProvider Local { get; } = new CacheStorageProvider(ApplicationData.Current.LocalCacheFolder);

        private HttpClient _HttpClient;

        /// <summary>
        /// Gets the <see cref="Windows.Web.Http.HttpClient"/> instance to use to download PDF documents
        /// </summary>
        [NotNull]
        private HttpClient HttpClient
        {
            get
            {
                if (_HttpClient == null)
                {
                    _HttpClient = new HttpClient();
                    _HttpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_4) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3710.0 Safari/537.36");
                }

                return _HttpClient;
            }
        }

        /// <summary>
        /// The synchronization mutex for the remote cache APIs
        /// </summary>
        [NotNull]
        private readonly AsyncMutex CacheMutex = new AsyncMutex();

        /// <inheritdoc/>
        public async Task<StorageFile> GetRemoteFileAsync(string url)
        {
            string filename = $"{url.BernsteinHash()}";
            if (!(await ApplicationData.Current.TemporaryFolder.TryGetItemAsync(filename) is StorageFile file))
            {
                using (IInputStream input = await HttpClient.GetInputStreamAsync(new Uri(url)))
                using (Stream stream = input.AsStreamForRead())
                using (await CacheMutex.LockAsync())
                {
                    file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                    using (Stream output = await file.OpenStreamForWriteAsync())
                    {
                        await stream.CopyToAsync(output);
                    }
                }
            }

            return file;
        }

        /// <summary>
        /// The default file extension for all cached files
        /// </summary>
        private const string FileExtension = ".json";

        /// <summary>
        /// A <see langword="class"/> that handles cached items on a specific storage location
        /// </summary>
        private sealed class CacheStorageProvider : ICacheStorageProvider
        {
            /// <summary>
            /// The synchronization mutex for the cache storage provider
            /// </summary>
            [NotNull]
            private readonly AsyncMutex CacheMutex = new AsyncMutex();

            /// <summary>
            /// The dictionary with the cached items
            /// </summary>
            [NotNull]
            private readonly Dictionary<string, object> CacheMap = new Dictionary<string, object>();

            /// <summary>
            /// The target folder to use to store and retrieve the cached values
            /// </summary>
            [NotNull]
            private readonly StorageFolder CacheFolder;

            /// <summary>
            /// Creates a new <see cref="CacheStorageProvider"/> instance that works on a specific <see cref="StorageFolder"/>
            /// </summary>
            /// <param name="folder">The target <see cref="StorageFolder"/> instance to use to store the cached values</param>
            public CacheStorageProvider([NotNull] StorageFolder folder) => CacheFolder = folder;

            // Gets the item key and filename
            [Pure]
            private static (string Key, string Filename) GetCacheKeys([NotNull] string key, [CanBeNull] string scope)
            {
                string name = scope == null
                    ? key.ToHex()
                    : $"{scope}_{key.ToHex()}";
                return (name, $"{name}{FileExtension}");
            }

            /// <inheritdoc/>
            public async Task<T> TryGetValueAsync<T>(string key, string scope = null) where T : class, new()
            {
                using (await CacheMutex.LockAsync())
                {
                    // Try to get the value from the runtime cache
                    var (hex, filename) = GetCacheKeys(key, scope);
                    if (CacheMap.TryGetValue(hex, out object value))
                        if (value is T result)
                            return result;

                    // Fallback to disk
                    if (await CacheFolder.TryGetItemAsync(filename) is IStorageFile file)
                    {
                        string json = await FileIO.ReadTextAsync(file);
                        T result = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<T>(json);
                        CacheMap.AddOrUpdate(hex, result); // Store in runtime cache for faster access in the future
                        return result;
                    }

                    return null;
                }
            }

            /// <inheritdoc/>
            public async Task<IReadOnlyList<T>> TryGetValuesAsync<T>(string scope) where T : class, new()
            {
                using (await CacheMutex.LockAsync())
                {
                    IReadOnlyList<StorageFile> files = await CacheFolder.GetFilesAsync();
                    return await Task.WhenAll(files.Where(file => file.DisplayName.StartsWith($"{scope}_")).Select(async file =>
                    {
                        string
                            hex = Regex.Match(file.DisplayName, $"{scope}_([0-9A-F]+)$").Groups[1].Value,
                            json = await FileIO.ReadTextAsync(file);
                        T result = string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<T>(json);
                        CacheMap.AddOrUpdate(hex, result); // Store in runtime cache for faster access in the future
                        return result;
                    }));
                }
            }

            /// <inheritdoc/>
            public async Task SetValueAsync<T>(string key, T value, string scope = null) where T : class, new()
            {
                using (await CacheMutex.LockAsync())
                {
                    var (hex, filename) = GetCacheKeys(key, scope);
                    StorageFile file = await CacheFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(value));
                    CacheMap.AddOrUpdate(hex, value);
                }
            }

            /// <inheritdoc/>
            public async Task DeleteValueAsync(string key, string scope = null)
            {
                using (await CacheMutex.LockAsync())
                {
                    var (hex, filename) = GetCacheKeys(key, scope);
                    if (await CacheFolder.TryGetItemAsync(filename) is StorageFile file)
                    {
                        await file.DeleteAsync();
                        CacheMap.Remove(hex);
                    }
                }
            }

            /// <inheritdoc/>
            public async Task DeleteValuesAsync(string scope = null)
            {
                using (await CacheMutex.LockAsync())
                {
                    IReadOnlyList<StorageFile> files = await CacheFolder.GetFilesAsync();
                    await Task.WhenAll(files.Where(file => file.DisplayName.StartsWith($"{scope}_")).Select(file =>
                    {
                        string hex = Regex.Match(file.DisplayName, $"{scope}_([0-9A-F]+)$").Groups[1].Value;
                        if (CacheMap.ContainsKey(hex)) CacheMap.Remove(hex);
                        return file.DeleteAsync().AsTask();
                    }));
                }
            }
        }
    }
}
