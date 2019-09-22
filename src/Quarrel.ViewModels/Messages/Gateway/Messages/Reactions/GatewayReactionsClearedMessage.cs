using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayReactionClearedMessage
    {
        public GatewayReactionClearedMessage(string messageId, string channelId)
        {
            MessageId = messageId;
            ChannelId = channelId;
        }

        public string MessageId { get; }

        public string ChannelId { get; }
    }
}
