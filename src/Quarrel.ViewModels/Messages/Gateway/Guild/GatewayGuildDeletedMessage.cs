using DiscordAPI.Gateway.DownstreamEvents;

namespace Quarrel.ViewModels.Messages.Gateway.Guild
{
    public class GatewayGuildDeletedMessage
    {
        public GatewayGuildDeletedMessage(GuildDelete guild)
        {
            Guild = guild;
        }

        public GuildDelete Guild { get; }
    }
}
