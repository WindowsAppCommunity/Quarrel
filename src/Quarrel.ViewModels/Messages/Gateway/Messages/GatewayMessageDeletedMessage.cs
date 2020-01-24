namespace Quarrel.ViewModels.Messages.Gateway
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
