// Quarrel © 2022

namespace Quarrel.Messages.Discord.Reactions
{
    public class AllReactionsRemovedMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllReactionsRemovedMessage"/> class.
        /// </summary>
        /// <param name="channelId">The channel id of the message.</param>
        /// <param name="messageId">The id of the message.</param>
        public AllReactionsRemovedMessage(ulong channelId, ulong messageId)
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
