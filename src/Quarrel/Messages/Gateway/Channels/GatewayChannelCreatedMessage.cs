using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarSymbols;
using Quarrel.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayGuildChannelCreatedMessage
    {
        public GatewayGuildChannelCreatedMessage(GuildChannel channel)
        {
            Channel = channel;
        }

        public GuildChannel Channel { get; }
    }
}
