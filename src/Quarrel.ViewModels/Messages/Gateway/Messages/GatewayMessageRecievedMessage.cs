using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayMessageRecievedMessage
    {
        public GatewayMessageRecievedMessage(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}
