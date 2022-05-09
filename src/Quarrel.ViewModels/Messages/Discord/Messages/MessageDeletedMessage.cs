// Quarrel © 2022

namespace Quarrel.Messages.Discord.Messages
{
    public class MessageDeletedMessage
    {
        public MessageDeletedMessage(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        public ulong ChannelId { get; }

        public ulong MessageId { get; }
    }
}
