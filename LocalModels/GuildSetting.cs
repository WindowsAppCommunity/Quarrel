using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public class GuildSetting
    {
        public GuildSetting(SharedModels.GuildSetting input)
        {
            raw = input;

            foreach (var channel in raw.ChannelOverrides)
            {
                channelOverrides.Add(channel.Channel_Id, channel);
            }
        }

        public Dictionary<string, ChannelOverride> channelOverrides = new Dictionary<string, ChannelOverride>();
        public SharedModels.GuildSetting raw = new SharedModels.GuildSetting();
    }
}
