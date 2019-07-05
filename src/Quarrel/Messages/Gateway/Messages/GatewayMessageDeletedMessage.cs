using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayMessageDeletedMessage
    {
        public GatewayMessageDeletedMessage(string channelId, string messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        public string ChannelId { get; }

        public string MessageId { get; }
    }
}
