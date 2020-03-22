// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    /// <summary>
    /// Manages the all channels the client has access to.
    /// </summary>
    public interface IChannelsService
    {
        /// <summary>
        /// Gets a hashed collection of all channels in loaded by the client, by id.
        /// </summary>
        IDictionary<string, BindableChannel> AllChannels { get; }

        /// <summary>
        /// Gets a hashed collection all channel's settings, by id.
        /// </summary>
        IDictionary<string, ChannelOverride> ChannelSettings { get; }

        /// <summary>
        /// Gets the currently open channel.
        /// </summary>
        BindableChannel CurrentChannel { get; }

        /// <summary>
        /// Gets a channel by id.
        /// </summary>
        /// <param name="channelId">The id of the channel.</param>
        /// <returns>The <see cref="BindableChannel"/> with id <paramref name="channelId"/>, or null.</returns>
        BindableChannel GetChannel(string channelId);
    }
}
