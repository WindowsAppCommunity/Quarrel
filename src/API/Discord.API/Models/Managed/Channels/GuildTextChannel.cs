// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    /// <summary>
    /// A text channel in a guild managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildTextChannel : Channel, IGuildTextChannel
    {
        internal GuildTextChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
            base(restChannel, context)
        {
            guildId = guildId ?? restChannel.GuildId;

            Guard.IsNotNull(restChannel.SlowModeDelay, nameof(restChannel.SlowModeDelay));
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(guildId, nameof(guildId));

            Topic = restChannel.Topic;
            IsNSFW = restChannel.IsNSFW;
            SlowModeDelay = restChannel.SlowModeDelay.Value;
            LastMessageId = restChannel.LastMessageId;
            CategoryId = restChannel.CategoryId;
            Position = restChannel.Position.Value;
            GuildId = guildId.Value;
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
        public int Position { get; private set; }

        /// <inheritdoc/>
        public ulong GuildId { get; private set; }

        /// <inheritdoc/>
        public int? MentionCount { get; internal set; }

        /// <inheritdoc/>
        public ulong? LastMessageId { get; internal set; }

        /// <inheritdoc/>
        public ulong? LastReadMessageId { get; internal set; }

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

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Topic = Topic;
            restChannel.IsNSFW = IsNSFW;
            restChannel.SlowModeDelay = SlowModeDelay;
            restChannel.LastMessageId = LastReadMessageId;
            restChannel.CategoryId = CategoryId;
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            return restChannel;
        }
    }
}
