// Copyright (c) Quarrel. All rights reserved.

using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Quarrel.ViewModels.Services.Discord.Presence
{
    /// <summary>
    /// Manages the presence of all users.
    /// </summary>
    public class PresenceService : IPresenceService
    {
        private IDictionary<string, DiscordAPI.Models.Presence> _presences = new ConcurrentDictionary<string, DiscordAPI.Models.Presence>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PresenceService"/> class.
        /// </summary>
        public PresenceService()
        {
            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, m =>
            {
                if (_presences.ContainsKey(m.UserId))
                {
                    _presences[m.UserId] = m.Presence;
                }
                else
                {
                    _presences.Add(m.UserId, m.Presence);
                }
            });
        }

        /// <inheritdoc/>
        public DiscordAPI.Models.Presence GetUserPrecense(string userId)
        {
            return _presences.TryGetValue(userId, out DiscordAPI.Models.Presence presence) ? presence : null;
        }

        /// <inheritdoc/>
        public void UpdateUserPrecense(string userId, DiscordAPI.Models.Presence presence)
        {
            Messenger.Default.Send(new GatewayPresenceUpdatedMessage(userId, presence));
        }
    }
}
