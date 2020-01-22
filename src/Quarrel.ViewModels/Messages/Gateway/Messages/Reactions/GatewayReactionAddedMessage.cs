using DiscordAPI.Models;

namespace Quarrel.Messages.Gateway
{
    public sealed class GatewayReactionAddedMessage
    {
        public GatewayReactionAddedMessage(string messageId, string channelId, Emoji emoji)
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
