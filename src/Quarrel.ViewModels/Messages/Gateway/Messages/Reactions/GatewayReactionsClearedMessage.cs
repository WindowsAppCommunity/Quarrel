namespace Quarrel.ViewModels.Messages.Gateway
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
