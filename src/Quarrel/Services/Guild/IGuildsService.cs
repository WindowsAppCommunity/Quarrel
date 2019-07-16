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
        Dictionary<string, BindableChannel> CurrentChannels { get; }
        Dictionary<string, BindableGuild> Guilds { get; }

        BindableChannel GetChannel(string channelId);
    }
}
