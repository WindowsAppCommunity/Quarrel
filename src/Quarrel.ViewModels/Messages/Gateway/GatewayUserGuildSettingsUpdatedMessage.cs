using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
