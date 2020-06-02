// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Guilds;
using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;

namespace DiscordAPI.Models
{
    /// <summary>
    /// Extensions for the <see cref="GuildSync"/> event.
    /// </summary>
    internal static class GuildSyncExtensions
    {
        /// <summary>
        /// Stores all data from the <see cref="GuildSync"/> event.
        /// </summary>
        /// <param name="sync"><see cref="GuildSync"/> event to cache data from.</param>
        public static void Cache(this GuildSync sync)
        {
            foreach (var presence in sync.Presences)
            {
                Messenger.Default.Send(new GatewayPresenceUpdatedMessage(presence.User.Id, presence));
            }
        }
    }
}
