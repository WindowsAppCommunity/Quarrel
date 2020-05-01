using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayUserGuildSettingsUpdatedMessage
    {
        public GuildSetting Settings { get; }

        public GatewayUserGuildSettingsUpdatedMessage(GuildSetting settings)
        {
            Settings = settings;
        }
    }
}
