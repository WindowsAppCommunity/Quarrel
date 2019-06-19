using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Helpers
{
    internal static class Constants
    {
        /// <summary>
        /// A <see langword="class"/> with some commonly used values for the cache services
        /// </summary>
        public static class Cache
        {
            /// <summary>
            /// A <see langword="class"/> with some commonly used keys for cached items
            /// </summary>
            public static class Keys
            {
                public const string GuildList = nameof(GuildList);
                public const string ChannelList = nameof(ChannelList);
                public const string GuildSettings = nameof(GuildSettings);
                public const string ChannelSettings = nameof(ChannelSettings);
                public const string ReadState = nameof(ReadState);
            }
        }
    }
}
