// Special thanks to Sergio Pedri for the basis of this design
// Copyright (c) Quarrel. All rights reserved.

using JetBrains.Annotations;
using Quarrel.ViewModels.Services.Cache.Persistent;
using Quarrel.ViewModels.Services.Cache.Runtime;

namespace Quarrel.ViewModels.Services.Cache
{
    /// <summary>
    /// The default <see langword="interface"/> for a comprehensive cache service, both runtime and persistent.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets the <see cref="IRuntimeCacheService"/> instance that only stores items at runtime.
        /// </summary>
        [NotNull]
        IRuntimeCacheService Runtime { get; }

        /// <summary>
        /// Gets the <see cref="IPersistentCacheService"/> instance that stores items in a persistent way.
        /// </summary>
        [NotNull]
        IPersistentCacheService Persistent { get; }
    }
}
