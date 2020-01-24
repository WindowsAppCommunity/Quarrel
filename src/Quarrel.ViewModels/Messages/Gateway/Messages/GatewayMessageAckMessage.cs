namespace Quarrel.ViewModels.Messages.Gateway
{
    public sealed class GatewayMessageAckMessage
    {
        public GatewayMessageAckMessage(string channelId, string messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        public string ChannelId { get; }

        public string MessageId { get; }
    }
}
