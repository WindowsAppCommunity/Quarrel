// Quarrel © 2022

namespace Quarrel.Client.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for text channels.
    /// </summary>
    public interface IMessageChannel : IChannel
    {
        /// <summary>
        /// The number of unread mentions the current user has in the channels.
        /// </summary>
        int? MentionCount { get; set; }

        /// <summary>
        /// The id of the last message in the channel.
        /// </summary>
        ulong? LastMessageId { get; set; }

        /// <summary>
        /// The id of the last message in the channel the current user has read.
        /// </summary>
        ulong? LastReadMessageId { get; set; }

        /// <summary>
        /// Gets whether or not the current user has muted the channel.
        /// </summary>
        bool IsMuted { get; }

        /// <summary>
        /// Gets whether or not the current user has unread messages in the channel.
        /// </summary>
        bool IsUnread { get; }
    }
}
