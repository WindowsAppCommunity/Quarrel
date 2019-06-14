// Special thanks to Sergio Pedri for the basis of this design

using System;
using System.Threading.Tasks;
using Windows.Storage;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Quarrel.Services.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for services that require access tokens to be initialized
    /// </summary>
    /// <typeparam name="T">The type of client info model used by the service</typeparam>
    internal abstract class ServiceWithAccessTokensBase<T> : IInitializableService where T : class, new()
    {
        /// <summary>
        /// Gets a mutex to avoid race conditions while performing operations in a given service
        /// </summary>
        [NotNull]
        protected readonly AsyncMutex RestMutex = new AsyncMutex();

        /// <summary>
        /// Gets the loaded client tokens for the current service
        /// </summary>
        [CanBeNull]
        protected T ClientInfo { get; private set; }

        /// <summary>
        /// Gets the path for the access tokens .json file, relative to "ms-appx:///Assets"
        /// </summary>
        [NotNull]
        protected abstract string Path { get; }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            using (await RestMutex.LockAsync())
            {
                // Ensure the client tokens are loaded
                if (ClientInfo != null) return;
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{Path}"));
                string json = await FileIO.ReadTextAsync(file);
                ClientInfo = JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Error loading the APIs tokens");

                // Additional initialization
                OnInitialize();
            }
        }

        /// <summary>
        /// Performs additional, service specific initializaiton operations
        /// </summary>
        protected abstract void OnInitialize();
    }
}
