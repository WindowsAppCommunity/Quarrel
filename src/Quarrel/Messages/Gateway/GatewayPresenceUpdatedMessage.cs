using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayPresenceUpdatedMessage
    {
        public GatewayPresenceUpdatedMessage(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}
