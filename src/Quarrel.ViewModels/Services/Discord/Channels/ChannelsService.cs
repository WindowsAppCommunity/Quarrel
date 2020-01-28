﻿using DiscordAPI.Models;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    public class ChannelsService : IChannelsService
    {
        public ChannelsService()
        {
            Messenger.Default.Register<ChannelNavigateMessage>(this, m =>
            {
                CurrentChannel = m.Channel;
            });
        }
        public BindableChannel GetChannel(string channelId)
        {
            return AllChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }
        public BindableChannel CurrentChannel { get; private set; }
        public IDictionary<string, BindableChannel> AllChannels { get; } = new ConcurrentDictionary<string, BindableChannel>();
        public ConcurrentDictionary<string, ChannelOverride> ChannelSettings { get; } =
            new ConcurrentDictionary<string, ChannelOverride>();
    }
}
