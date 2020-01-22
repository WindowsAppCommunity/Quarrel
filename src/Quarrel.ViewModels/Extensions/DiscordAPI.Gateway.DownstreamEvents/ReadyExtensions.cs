using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using Quarrel.Services.Users;
using Quarrel.ViewModels.Helpers;
using Quarrel.ViewModels.Models.Bindables;
using System.Collections.Generic;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    internal static class ReadyExtentions
    {
        private static ICurrentUsersService currentUsersService = SimpleIoc.Default.GetInstance<ICurrentUsersService>();
        private static ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private static IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        // TODO: Remove Cache usage
        public static void Cache(this Ready ready)
        {
            #region Settings

            foreach (var gSettings in ready.GuildSettings)
            {
                currentUsersService.GuildSettings.AddOrUpdate(gSettings.GuildId ?? "DM", gSettings);
                
                foreach (var cSettings in gSettings.ChannelOverrides)
                {
                    currentUsersService.ChannelSettings.AddOrUpdate(cSettings.ChannelId, cSettings);
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
                currentUsersService.Friends.AddOrUpdate(friend.Id, new BindableFriend(friend));
            }

            #endregion

            #region Current User

            discordService.CurrentUser = ready.User;

            #endregion
        }
    }
}
