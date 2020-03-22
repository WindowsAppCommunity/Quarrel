// Copyright (c) Quarrel. All rights reserved.

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
