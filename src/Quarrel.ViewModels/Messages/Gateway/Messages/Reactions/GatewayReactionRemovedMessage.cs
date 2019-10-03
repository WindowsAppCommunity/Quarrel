using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayReactionRemovedMessage
    {
        public GatewayReactionRemovedMessage(string messageId, string channelId, Emoji emoji)
        {
            MessageId = messageId;
            ChannelId = channelId;
            Emoji = emoji;
        }

        public string MessageId { get; }

        public string ChannelId { get; }

        public Emoji Emoji { get; }
    }
}
