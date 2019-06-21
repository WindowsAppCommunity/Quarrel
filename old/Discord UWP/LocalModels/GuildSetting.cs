using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;

namespace Quarrel.LocalModels
{
    public class GuildSetting
    {
        /// <summary>
        /// Initialize GuildSettings from API model
        /// </summary>
        /// <param name="input"></param>
        public GuildSetting(DiscordAPI.SharedModels.GuildSetting input)
        {
            raw = input;
            
            // If guild has channel overrides
            if (raw.ChannelOverrides != null)
            {
                // Put each override in an Id'd Dictionary
                foreach (var channel in raw.ChannelOverrides)
                {
                    channelOverrides.Add(channel.Channel_Id, channel);
                }
            }
        }

        /// <summary>
        /// Channel overrides by Id
        /// </summary>
        public Dictionary<string, ChannelOverride> channelOverrides = new Dictionary<string, ChannelOverride>();

        /// <summary>
        /// API object
        /// </summary>
        public DiscordAPI.SharedModels.GuildSetting raw = new DiscordAPI.SharedModels.GuildSetting();
    }
}
