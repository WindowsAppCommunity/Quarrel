using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
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
