using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.Models;
using Quarrel.Models.Bindables;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayReactionAddedMessage
    {
        public GatewayReactionAddedMessage(string messageId, string channelId, Emoji emoji, bool me)
        {
            MessageId = messageId;
            ChannelId = channelId;
            Emoji = emoji;
            Me = me;
        }

        public string MessageId { get; }

        public string ChannelId { get; }

        public Emoji Emoji { get; }

        public bool Me { get; }
    }
}
