namespace Quarrel.ViewModels.Messages.Gateway.Guild
{
    public class GatewayGuildCreatedMessage
    {
        public GatewayGuildCreatedMessage(DiscordAPI.Models.Guild guild)
        {
            Guild = guild;
        }

        public DiscordAPI.Models.Guild Guild { get; }
    }
}
