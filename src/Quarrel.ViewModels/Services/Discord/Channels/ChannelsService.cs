using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    public class ChannelsService : IChannelsService
    {
        public BindableChannel GetChannel(string channelId)
        {
            return CurrentChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }

        public IDictionary<string, BindableChannel> CurrentChannels { get; } = new ConcurrentDictionary<string, BindableChannel>();
        public IDictionary<string, BindableChannel> AllChannels { get; } = new ConcurrentDictionary<string, BindableChannel>();

        public ConcurrentDictionary<string, ChannelOverride> ChannelSettings { get; } =
            new ConcurrentDictionary<string, ChannelOverride>();
    }
}
