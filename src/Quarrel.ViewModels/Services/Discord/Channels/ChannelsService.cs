// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Messages.Navigation;
using Quarrel.ViewModels.Models.Bindables.Channels;
using Quarrel.ViewModels.Services.Analytics;
using Quarrel.ViewModels.Services.DispatcherHelper;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    /// <summary>
    /// Manages the all channels the client has access to.
    /// </summary>
    public class ChannelsService : IChannelsService
    {
        private MainViewModel _mainViewModel = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelsService"/> class.
        /// </summary>
        public ChannelsService()
        {
        }

        /// <inheritdoc/>
        public BindableChannel CurrentChannel
        {
            get => MainViewModel.CurrentChannel;
            set => MainViewModel.CurrentChannel = value;
        }

        private MainViewModel MainViewModel => _mainViewModel ?? (_mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>());

        /// <inheritdoc/>
        public BindableChannel GetChannel(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return MainViewModel.AllChannels.TryGetValue(channelId, out BindableChannel channel) ? channel : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateChannel(string channelId, BindableChannel channel)
        {
            if (channelId == null)
            {
                return;
            }

            MainViewModel.AllChannels.AddOrUpdate(channelId, channel);
        }

        /// <inheritdoc/>
        public void RemoveChannel(string channelId)
        {
            MainViewModel.AllChannels.Remove(channelId);
        }

        /// <inheritdoc/>
        public ChannelOverride GetChannelSettings(string channelId)
        {
            if (channelId == null)
            {
                return null;
            }

            return MainViewModel.ChannelSettings.TryGetValue(channelId, out ChannelOverride channelOverride) ? channelOverride : null;
        }

        /// <inheritdoc/>
        public void AddOrUpdateChannelSettings(string channelId, ChannelOverride channelOverride)
        {
            if (channelId == null)
            {
                return;
            }

            MainViewModel.ChannelSettings.AddOrUpdate(channelId, channelOverride);
        }
    }
}
