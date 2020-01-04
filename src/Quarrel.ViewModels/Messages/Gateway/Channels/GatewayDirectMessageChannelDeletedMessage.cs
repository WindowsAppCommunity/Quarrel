using DiscordAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quarrel.ViewModels.Messages.Gateway.Channels
{
    public sealed class GatewayDirectMessageChannelDeletedMessage
    {
        public GatewayDirectMessageChannelDeletedMessage(DirectMessageChannel channel)
        {
            Channel = channel;
        }

        public DirectMessageChannel Channel { get; }
    }
}
