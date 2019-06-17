// Special thanks to Sergio Pedri for the basis of this design

using Quarrel.Services.Cache.Persistent;
using Quarrel.Services.Cache.Runtime;

namespace Quarrel.Services.Cache
{
    /// <summary>
    /// A <see langword="class"/> that provides caching functionalities, both at runtime and in a persistent way
    /// </summary>
    public sealed class CacheService : ICacheService
    {
        /// <inheritdoc/>
        public IRuntimeCacheService Runtime { get; } = new RuntimeCacheService();

        /// <inheritdoc/>
        public IPersistentCacheService Persistent { get; } = new PersistentCacheService();
    }
}
