// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;

namespace Quarrel.Client.Models.Channels
{
    /// <summary>
    /// A text channel in a guild managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class GuildTextChannel : GuildChannel, IGuildTextChannel
    {
        private ulong? _lastMessageId;
        private ulong? _lastReadMessageId;

        internal GuildTextChannel(JsonChannel restChannel, ulong? guildId, QuarrelClient context) :
            base(restChannel, guildId, context)
        {
            Guard.IsNotNull(restChannel.SlowModeDelay, nameof(restChannel.SlowModeDelay));

            Topic = restChannel.Topic;
            IsNSFW = restChannel.IsNSFW;
            SlowModeDelay = restChannel.SlowModeDelay.Value;
            LastMessageId = restChannel.LastMessageId;
            CategoryId = restChannel.CategoryId;
        }

        /// <inheritdoc/>
        public string? Topic { get; private set; }

        /// <inheritdoc/>
        public bool? IsNSFW { get; private set; }

        /// <inheritdoc/>
        public int? SlowModeDelay { get; private set; }

        /// <inheritdoc/>
        public ulong? CategoryId { get; private set; }

        /// <inheritdoc/>
        public int? MentionCount { get; internal set; }

        /// <inheritdoc/>
        public ulong? LastMessageId
        {
            get => _lastMessageId;
            internal set => UpdateItem(ref _lastMessageId, value);
        }

        /// <inheritdoc/>
        public ulong? LastReadMessageId
        {
            get => _lastReadMessageId;
            internal set => UpdateItem(ref _lastReadMessageId, value);
        }

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

        internal override void PrivateUpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            base.PrivateUpdateFromJsonChannel(jsonChannel);
            Topic = jsonChannel.Topic ?? Topic;
            IsNSFW = jsonChannel.IsNSFW ?? IsNSFW;
            SlowModeDelay = jsonChannel.SlowModeDelay ?? SlowModeDelay;
            CategoryId = jsonChannel.CategoryId ?? CategoryId;
        }

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            restChannel.Topic = Topic;
            restChannel.IsNSFW = IsNSFW;
            restChannel.SlowModeDelay = SlowModeDelay;
            restChannel.LastMessageId = LastReadMessageId;
            restChannel.CategoryId = CategoryId;
            return restChannel;
        }
    }
}
