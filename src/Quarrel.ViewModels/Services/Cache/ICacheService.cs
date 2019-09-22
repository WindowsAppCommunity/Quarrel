// Special thanks to Sergio Pedri for the basis of this design

using JetBrains.Annotations;
using Quarrel.Services.Cache.Persistent;
using Quarrel.Services.Cache.Runtime;

namespace Quarrel.Services.Cache
{
    /// <summary>
    /// The default <see langword="interface"/> for a comprehensive cache service, both runtime and persistent
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets the <see cref="IRuntimeCacheService"/> instance that only stores items at runtime
        /// </summary>
        [NotNull]
        IRuntimeCacheService Runtime { get; }

        /// <summary>
        /// Gets the <see cref="IPersistentCacheService"/> instance that stores items in a persistent way
        /// </summary>
        [NotNull]
        IPersistentCacheService Persistent { get; }
    }
}
