// Quarrel © 2022

using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Client.Models.Channels.Abstract
{
    /// <summary>
    /// The base class for private channels.
    /// </summary>
    public abstract class PrivateChannel : Channel, IPrivateChannel
    {
        internal PrivateChannel(JsonChannel restChannel, QuarrelClient context) :
            base(restChannel, context)
        {
            LastMessageId = restChannel.LastMessageId;
            RTCRegion = restChannel.RTCRegion;
        }

        /// <inheritdoc/>
        public int? MentionCount { get; private set; }

        /// <inheritdoc/>
        public ulong? LastMessageId { get; private set; }

        /// <inheritdoc/>
        public ulong? LastReadMessageId { get; private set; }

        /// <inheritdoc/>
        public string? RTCRegion { get; private set; }

        /// <inheritdoc/>
        public bool IsUnread => LastMessageId > LastReadMessageId;

        int? IMessageChannel.MentionCount
        {
            get => MentionCount;
            set => MentionCount = value;
        }

        ulong? IMessageChannel.LastMessageId
        {
            get => LastMessageId;
            set => LastMessageId = value;
        }

        ulong? IMessageChannel.LastReadMessageId
        {
            get => LastReadMessageId;
            set => LastReadMessageId = value;
        }
    }
}
