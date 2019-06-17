// Special thanks to Sergio Pedri for the basis of this design

using Quarrel.Services.Rest;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Services.Gateway;
using Quarrel.Services.Cache;

namespace Quarrel.Services
{
    internal class ServicesManager
    {
        /// <summary>
        /// Gets the default <see cref="IDiscordService"/> implementation (see <see cref="DiscordService"/>)
        /// </summary>
        [NotNull]
        public static IDiscordService Discord { get; } = new DiscordService();

        /// <summary>
        /// Gets the default <see cref="IDiscordService"/> implementation (see <see cref="DiscordService"/>)
        /// </summary>
        [NotNull]
        public static ICacheService Cache { get; } = new CacheService();

    }
}
