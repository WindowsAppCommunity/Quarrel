using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayMessageUpdatedMessage
    {
        public GatewayMessageUpdatedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
