namespace Quarrel.ViewModels.Messages.Gateway.Guild
{
    public class GatewayGuildUpdatedMessage
    {
        public GatewayGuildUpdatedMessage(DiscordAPI.Models.Guilds.Guild guild)
        {
            Guild = guild;
        }

        public DiscordAPI.Models.Guilds.Guild Guild { get; }
    }
}
