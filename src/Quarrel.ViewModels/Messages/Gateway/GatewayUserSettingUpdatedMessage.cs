using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayUserSettingsUpdatedMessage
    {
        public UserSettings Settings { get; }

        public GatewayUserSettingsUpdatedMessage(UserSettings settings)
        {
            Settings = settings;
        }
    }
}
