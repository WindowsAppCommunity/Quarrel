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
        ReadOnlyDictionary<string, BindableChannel> CurrentChannels { get; }
        void RegisterChannel(BindableChannel channel, string channelId);
        BindableChannel GetChannel(string channelId);
    }
}
