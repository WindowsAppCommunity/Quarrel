// Quarrel © 2022

namespace Quarrel.Messages.Discord.Messages
{
    /// <summary>
    /// A message sent when a message is marked read.
    /// </summary>
    public class MessageMarkedReadMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDeletedMessage"/> class.
        /// </summary>
        /// <param name="channelId">The channel id of the message marked read.</param>
        /// <param name="messageId">The id of the message mark read.</param>
        public MessageMarkedReadMessage(ulong channelId, ulong messageId)
        {
            ChannelId = channelId;
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the channel id of message marked read.
        /// </summary>
        public ulong ChannelId { get; }

        /// <summary>
        /// Gets the id of message marked read.
        /// </summary>
        public ulong MessageId { get; }
    }
}
