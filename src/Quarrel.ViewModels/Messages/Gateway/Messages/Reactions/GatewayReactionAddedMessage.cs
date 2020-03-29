using DiscordAPI.Models;

namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayReactionAddedMessage
    {
        public GatewayReactionAddedMessage(string messageId, string channelId, Emoji emoji, string userId)
        {
            MessageId = messageId;
            ChannelId = channelId;
            Emoji = emoji;
            UserId = userId;
        }

        public string MessageId { get; }

        public string ChannelId { get; }

        public Emoji Emoji { get; }

        public string UserId { get; }
    }
}
