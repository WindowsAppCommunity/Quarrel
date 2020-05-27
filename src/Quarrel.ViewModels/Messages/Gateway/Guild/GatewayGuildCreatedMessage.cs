namespace Quarrel.ViewModels.Messages.Gateway.Guild
{
    public class GatewayGuildCreatedMessage
    {
        public GatewayGuildCreatedMessage(DiscordAPI.Models.Guilds.Guild guild)
        {
            Guild = guild;
        }

        public DiscordAPI.Models.Guilds.Guild Guild { get; }
    }
}
