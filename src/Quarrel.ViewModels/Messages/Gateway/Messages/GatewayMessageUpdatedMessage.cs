using DiscordAPI.Models;
using DiscordAPI.Models.Messages;

namespace Quarrel.ViewModels.Messages.Gateway
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
