using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
