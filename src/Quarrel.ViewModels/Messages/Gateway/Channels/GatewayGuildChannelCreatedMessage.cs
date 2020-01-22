using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayChannelCreatedMessage
    {
        public GatewayChannelCreatedMessage(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }
    }
}
