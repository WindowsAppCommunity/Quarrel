using CommonServiceLocator;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using Quarrel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Quarrel.Helpers;
using DiscordAPI.Models;
using GalaSoft.MvvmLight.Ioc;
using Quarrel.Services.Cache;
using Quarrel.Services.Rest;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.Messages.Gateway;

namespace DiscordAPI.Gateway.DownstreamEvents
{
    internal static class ReadyExtentions
    {
        private static ICacheService cacheService = SimpleIoc.Default.GetInstance<ICacheService>();
        private static IDiscordService discordService = SimpleIoc.Default.GetInstance<IDiscordService>();
        // TODO: Remove Cache usage
        public static void Cache(this Ready ready)
        {
            #region Settings

            foreach (var gSettings in ready.GuildSettings)
            {
                cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.GuildSettings, gSettings, gSettings.GuildId);

                foreach (var cSettings in gSettings.ChannelOverrides)
                {
                    cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.ChannelSettings, cSettings, cSettings.ChannelId);
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
                cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Note, note.Value, note.Key);
            }

            #endregion

            #region Friends

            foreach (var friend in ready.Friends)
            {
                cacheService.Runtime.SetValue(Quarrel.Helpers.Constants.Cache.Keys.Friend, friend, friend.Id);
            }

            #endregion

            #region Current User

            discordService.CurrentUser = ready.User;

            #endregion
        }
    }
}
