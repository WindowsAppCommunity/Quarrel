using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Navigation
{
    public sealed class GuildNavigateMessage
    {
        public GuildNavigateMessage(string guildId)
        {
            GuildId = guildId;
        }

        public string GuildId { get; }
    }
}
