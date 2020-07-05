// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models;
using Quarrel.ViewModels.Models.Bindables.Channels;

namespace Quarrel.ViewModels.Services.Discord.Channels
{
    /// <summary>
    /// Manages the all channels the client has access to.
    /// </summary>
    public interface IChannelsService
    {
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

        /// <summary>
        /// Adds or updates a channel in the channel list.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="channel">The <see cref="BindableChannel"/> object.</param>
        void AddOrUpdateChannel(string channelId, BindableChannel channel);

        /// <summary>
        /// Removes a channel from the channel list.
        /// </summary>
        /// <param name="channelId">The channel's id</param>
        void RemoveChannel(string channelId);

        /// <summary>
        /// Gets a channel's settings.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <returns>The <see cref="ChannelOverride"/> for the channel.</returns>
        ChannelOverride GetChannelSettings(string channelId);

        /// <summary>
        /// Adds or updates a channel's settings.
        /// </summary>
        /// <param name="channelId">The channel's id.</param>
        /// <param name="channelOverride">The channel's new settings.</param>
        void AddOrUpdateChannelSettings(string channelId, ChannelOverride channelOverride);
    }
}
