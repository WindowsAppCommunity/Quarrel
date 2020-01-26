using GalaSoft.MvvmLight.Messaging;
using Quarrel.ViewModels.Messages.Gateway;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Services.Discord.Presence
{
    public class PresenceService : IPresenceService
    {
        public PresenceService()
        {
            Messenger.Default.Register<GatewayPresenceUpdatedMessage>(this, m =>
            {
                if (_Presences.ContainsKey(m.UserId))
                    _Presences[m.UserId] = m.Presence;
                else
                    _Presences.TryAdd(m.UserId, m.Presence);
            });
        }

        private ConcurrentDictionary<string, DiscordAPI.Models.Presence> _Presences = new ConcurrentDictionary<string, DiscordAPI.Models.Presence>();

        public DiscordAPI.Models.Presence GetUserPrecense(string userId)
        {
            return _Presences.TryGetValue(userId, out DiscordAPI.Models.Presence presence) ? presence : null;
        }

        public void UpdateUserPrecense(string userId, DiscordAPI.Models.Presence presence)
        {
            if (_Presences.ContainsKey(userId))
                _Presences[userId] = presence;
            else
                _Presences.TryAdd(userId, presence);
        }
    }
}
