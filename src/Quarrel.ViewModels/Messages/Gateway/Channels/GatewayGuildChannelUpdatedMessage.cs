// Copyright (c) Quarrel. All rights reserved.

using DiscordAPI.Models.Channels;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayGuildChannelUpdatedMessage
    {
        public GatewayGuildChannelUpdatedMessage(GuildChannel channel)
        {
            Channel = channel;
        }

        public GuildChannel Channel { get; }
    }
}
