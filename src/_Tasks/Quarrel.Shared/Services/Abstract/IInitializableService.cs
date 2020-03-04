// Copyright (c) Quarrel. All rights reserved.

using System.Threading.Tasks;

namespace Quarrel.Services.Abstract
{
    /// <summary>
    /// A base <see langword="interface"/> for a service that requires initialization before being used.
    /// </summary>
    internal interface IInitializableService
    {
        /// <summary>
        /// Initializes the services and performs all the necessary preliminary operations.
        /// </summary>
        /// <returns>The service.</returns>
        Task InitializeAsync();
    }
}
