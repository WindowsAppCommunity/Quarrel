// Special thanks to Sergio Pedri for this design from Legere
// Sergio's GitHub: https://github.com/Sergio0694
// Legere: https://www.microsoft.com/store/apps/9PHJRVCSKVJZ

namespace Quarrel.Services.Cache.Persistent
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the location of an item cached in the persistant cache.
    /// </summary>
    public enum CacheLocation
    {
        /// <summary>
        /// Roaming cache.
        /// </summary>
        Roaming,

        /// <summary>
        /// Local cache.
        /// </summary>
        Local,
    }
}
