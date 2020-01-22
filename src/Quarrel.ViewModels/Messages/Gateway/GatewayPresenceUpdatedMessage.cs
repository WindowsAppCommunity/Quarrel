using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayPresenceUpdatedMessage
    {
        public GatewayPresenceUpdatedMessage(string userId, Presence presence)
        {
            UserId = userId;
            Presence = presence;
        }

        public string UserId { get; }
        public Presence Presence { get; }
    }
}
