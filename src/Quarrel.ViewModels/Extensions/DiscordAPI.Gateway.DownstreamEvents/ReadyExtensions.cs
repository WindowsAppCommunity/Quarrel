// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Messages.Gateway;
using Quarrel.ViewModels.Models.Bindables;
using Quarrel.ViewModels.Services.Cache;
using Quarrel.ViewModels.Services.Discord.Channels;
using Quarrel.ViewModels.Services.Discord.CurrentUser;
using Quarrel.ViewModels.Services.Discord.Friends;
using Quarrel.ViewModels.Services.Discord.Guilds;
using Quarrel.ViewModels.Services.Discord.Rest;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    /// <summary>
    /// Extensions for the <see cref="Ready"/> class.
    /// </summary>
    internal static class ReadyExtensions
    {
        private static ICurrentUserService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>();
        private static IChannelsService channelsService = SimpleIoc.Default.GetInstance<IChannelsService>();
        private static ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private static IGuildsService guildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        private static IFriendsService friendsService = SimpleIoc.Default.GetInstance<IFriendsService>();
        private static IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();

        /// <summary>
        /// Stores all data from the <see cref="Ready"/> event.
        /// </summary>
        /// <param name="ready"><see cref="Ready"/> event to cache data from.</param>
        public static void Cache(this Ready ready)
        {
            // Cache Guild Settings
            foreach (var gSettings in ready.GuildSettings)
            {
                guildsService.GuildSettings.AddOrUpdate(gSettings.GuildId ?? "DM", gSettings);

                foreach (var cSettings in gSettings.ChannelOverrides)
                {
                    channelsService.ChannelSettings.AddOrUpdate(cSettings.ChannelId, cSettings);
                }
            }

            // Cache Presences
            foreach (var presence in ready.Presences)
            {
                Messenger.Default.Send<GatewayPresenceUpdatedMessage>(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
            }

            // Cache user notes
            foreach (var note in ready.Notes)
            {
                // TODO: Remove Cache usage
                cacheService.Runtime.SetValue(Constants.Cache.Keys.Note, note.Value, note.Key);
            }

            // Cache friends
            foreach (var friend in ready.Friends)
            {
                friendsService.Friends.AddOrUpdate(friend.Id, new BindableFriend(friend));
            }

            // Cache current user.
            discordService.CurrentUser = ready.User;
            currentUsersService.CurrentUser.Model = ready.User;
        }
    }
}
