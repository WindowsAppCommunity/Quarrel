using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayGuildChannelUpdatedMessage
    {
        public GatewayGuildChannelUpdatedMessage(GuildChannel channel)
        {
            Channel = channel;
        }

        public GuildChannel Channel { get; }
    }
}
