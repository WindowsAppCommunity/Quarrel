using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DiscordAPI.SharedModels;

namespace Quarrel.LocalModels
{
    public class GuildChannel
    {
        /// <summary>
        /// Create GuildChannel object from API model
        /// </summary>
        /// <param name="channel">API model</param>
        public GuildChannel(DiscordAPI.SharedModels.GuildChannel channel)
        {
            raw = channel;
        }

        /// <summary>
        /// Create GuildChannel from API model with missing guildId
        /// </summary>
        /// <param name="channel">API model</param>
        /// <param name="guildId">Guild ID</param>
        public GuildChannel(DiscordAPI.SharedModels.GuildChannel channel, string guildId)
        {
            raw = channel;
            raw.GuildId = guildId;
        }

        /// <summary>
        /// Get user permissions in channel
        /// </summary>
        public Permissions permissions => new Permissions(raw.GuildId, raw.Id);

        /// <summary>
        /// API Model object
        /// </summary>
        public DiscordAPI.SharedModels.GuildChannel raw;
    }
}
