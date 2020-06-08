// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

using Quarrel.Services.Cache.Persistent;
using Quarrel.Services.Cache.Runtime;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Cache.Persistent;
using Quarrel.ViewModels.Services.Cache.Runtime;

namespace Quarrel.Services.Cache
{
    /// <summary>
    /// A <see langword="class"/> that provides caching functionalities, both at runtime and in a persistent way.
    /// </summary>
    public sealed class CacheService : ICacheService
    {
        /// <inheritdoc/>
        public IRuntimeCacheService Runtime { get; } = new RuntimeCacheService();

        /// <inheritdoc/>
        public IPersistentCacheService Persistent { get; } = new PersistentCacheService();
    }
}
