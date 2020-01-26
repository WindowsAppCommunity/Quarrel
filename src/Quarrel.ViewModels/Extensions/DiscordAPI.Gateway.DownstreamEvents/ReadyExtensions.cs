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
    internal static class ReadyExtentions
    {
        private static ICurrentUserService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUserService>();
        private static IChannelsService channelsService = SimpleIoc.Default.GetInstance<IChannelsService>();
        private static ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private static IGuildsService guildsService = SimpleIoc.Default.GetInstance<IGuildsService>();
        private static IFriendsService friendsService = SimpleIoc.Default.GetInstance<IFriendsService>();
        private static IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        // TODO: Remove Cache usage
        public static void Cache(this Ready ready)
        {
            #region Settings

            foreach (var gSettings in ready.GuildSettings)
            {
                guildsService.GuildSettings.AddOrUpdate(gSettings.GuildId ?? "DM", gSettings);
                
                foreach (var cSettings in gSettings.ChannelOverrides)
                {
                    channelsService.ChannelSettings.AddOrUpdate(cSettings.ChannelId, cSettings);
                }
            }

            #endregion

            #region Presence

            foreach (var presence in ready.Presences)
            {
                Messenger.Default.Send<GatewayPresenceUpdatedMessage>(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
            }

            #endregion

            #region Notes

            foreach (var note in ready.Notes)
            {
                cacheService.Runtime.SetValue(Constants.Cache.Keys.Note, note.Value, note.Key);
            }

            #endregion

            #region Friends

            foreach (var friend in ready.Friends)
            {
                friendsService.Friends.AddOrUpdate(friend.Id, new BindableFriend(friend));
            }

            #endregion

            #region Current User

            discordService.CurrentUser = ready.User;

            #endregion
        }
    }
}
