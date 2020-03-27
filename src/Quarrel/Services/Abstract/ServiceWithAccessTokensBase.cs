// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace Quarrel.Services.Abstract
{
    /// <summary>
    /// A base <see langword="class"/> for services that require access tokens to be initialized.
    /// </summary>
    /// <typeparam name="T">The type of client info model used by the service.</typeparam>
    public abstract class ServiceWithAccessTokensBase<T> : IInitializableService
        where T : class, new()
    {
        /// <summary>
        /// Gets a semaphore slim to avoid race conditions while performing operations in a given service.
        /// </summary>
        [NotNull]
        private readonly SemaphoreSlim _restSemaphoreSlim = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Gets the loaded client tokens for the current service.
        /// </summary>
        [CanBeNull]
        protected T ClientInfo { get; private set; }

        /// <summary>
        /// Gets the path for the access tokens .json file, relative to "ms-appx:///Assets".
        /// </summary>
        [NotNull]
        protected abstract string Path { get; }

        /// <summary>
        /// Initializes the services and performs all the necessary preliminary operations.
        /// </summary>
        public async void Initialize()
        {
            await InitializeAsync();
        }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await _restSemaphoreSlim.WaitAsync();
            try
            {
                // Ensure the client tokens are loaded
                if (ClientInfo != null)
                {
                    return;
                }

                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{Path}"));
                string json = await FileIO.ReadTextAsync(file);
                ClientInfo = JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Error loading the APIs tokens");

                // Additional initialization
                OnInitialize();
            }
            finally
            {
                _restSemaphoreSlim.Release();
            }
        }

        /// <summary>
        /// Performs additional, service specific initializaiton operations.
        /// </summary>
        protected abstract void OnInitialize();
    }
}
