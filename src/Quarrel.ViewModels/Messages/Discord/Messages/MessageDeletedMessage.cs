// Quarrel © 2022

namespace Quarrel.Messages.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is deleted.
    /// </summary>
    public class MessageDeletedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDeletedMessage"/> class.
        /// </summary>
        /// <param name="channelId">The channel id of deleted message.</param>
        /// <param name="messageId">The id of the deleted message.</param>
        public MessageDeletedMessage(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the channel id of deleted message.
        /// </summary>
        public ulong ChannelId { get; }

        /// <summary>
        /// Gets the id of deleted message.
        /// </summary>
        public ulong MessageId { get; }
    }
}
