// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Channels;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayChannelCreatedMessage
    {
        public GatewayChannelCreatedMessage(Channel channel)
        {
            Channel = channel;
        }

        public Channel Channel { get; }
    }
}
