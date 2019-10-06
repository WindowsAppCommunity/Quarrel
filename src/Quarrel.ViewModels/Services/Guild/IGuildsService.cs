using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Models.Bindables;

namespace Quarrel.Services.Guild
{
    public interface IGuildsService
    {
        IDictionary<string, BindableChannel> CurrentChannels { get; }
        IDictionary<string, BindableGuild> Guilds { get; }
        string CurrentGuildId { get; }
        BindableGuild CurrentGuild { get; }

        BindableChannel GetChannel(string channelId);
    }
}
