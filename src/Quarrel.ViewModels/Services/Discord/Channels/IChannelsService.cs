using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    public interface IChannelsService
    {
        IDictionary<string, BindableChannel> AllChannels { get; }
        ConcurrentDictionary<string, ChannelOverride> ChannelSettings { get; }
        BindableChannel CurrentChannel { get; }
        BindableChannel GetChannel(string channelId);
    }
}
