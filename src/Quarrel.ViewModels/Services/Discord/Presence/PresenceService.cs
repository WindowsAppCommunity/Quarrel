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
                if (presences.ContainsKey(m.UserId))
                    presences[m.UserId] = m.Presence;
                else
                    presences.TryAdd(m.UserId, m.Presence);
            });
        }

        private ConcurrentDictionary<string, DiscordAPI.Models.Presence> presences = new ConcurrentDictionary<string, DiscordAPI.Models.Presence>();

        public DiscordAPI.Models.Presence GetUserPrecense(string userId)
        {
            return presences.TryGetValue(userId, out DiscordAPI.Models.Presence presence) ? presence : null;
        }

        public void UpdateUserPrecense(string userId, DiscordAPI.Models.Presence presence)
        {
            if (presences.ContainsKey(userId))
                presences[userId] = presence;
            else
                presences.TryAdd(userId, presence);
        }
    }
}
