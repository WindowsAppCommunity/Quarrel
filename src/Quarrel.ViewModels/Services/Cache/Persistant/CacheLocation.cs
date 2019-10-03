// Special thanks to Sergio Pedri for the basis of this design

namespace Quarrel.Services.Cache.Persistent
{
    /// <summary>
    /// An <see langword="enum"/> that indicates the location of an item cached in the persistant cache
    /// </summary>
    public enum CacheLocation
    {
        Roaming,
        Local
    }
}
