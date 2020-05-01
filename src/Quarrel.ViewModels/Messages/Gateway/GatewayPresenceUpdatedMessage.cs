using DiscordAPI.Models;

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
