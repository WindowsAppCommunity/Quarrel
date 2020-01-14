using System;
using System.Collections.Generic;
using System.Text;
using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public class GatewayGuildMemberListUpdatedMessage
    {
        public GuildMemberListUpdated GuildMemberListUpdated { get; set; }

        public GatewayGuildMemberListUpdatedMessage(GuildMemberListUpdated guildMemberListUpdated)
        {
            GuildMemberListUpdated = guildMemberListUpdated;
        }
    }
}
