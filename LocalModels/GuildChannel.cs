using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord_UWP.SharedModels;

namespace Discord_UWP.LocalModels
{
    public class GuildChannel
    {
        public GuildChannel(SharedModels.GuildChannel channel)
        {
            raw = channel;
            //GetPermissions(); Called to early
        }

        public GuildChannel(SharedModels.GuildChannel channel, string guildId)
        {
            raw = channel;
            raw.GuildId = guildId;
        }

        public Permissions permissions
        {
            get { return new Permissions(raw.GuildId, raw.Id); }
        }

        public SharedModels.GuildChannel raw;
    }
}
