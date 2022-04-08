﻿// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Managed.Channels;
using Discord.API.Models.Managed.Channels.Abstract;

namespace Discord.API.Models.Channels
{
    /// <summary>
    /// A text channel in a guild managed by a <see cref="DiscordClient"/>.
    /// </summary>
    public class GuildTextChannel : GuildChannel, IGuildTextChannel
    {
        internal GuildTextChannel(JsonChannel restChannel, ulong? guildId, DiscordClient context) :
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

        internal override void UpdateFromRestChannel(JsonChannel jsonChannel)
        {
            base.UpdateFromRestChannel(jsonChannel);
            Topic = jsonChannel.Topic ?? Topic;
            IsNSFW = jsonChannel.IsNSFW ?? IsNSFW;
            SlowModeDelay = jsonChannel.SlowModeDelay ?? SlowModeDelay;
            CategoryId = jsonChannel.CategoryId ?? CategoryId;
        }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Topic = Topic;
            restChannel.IsNSFW = IsNSFW;
            restChannel.SlowModeDelay = SlowModeDelay;
            restChannel.LastMessageId = LastReadMessageId;
            restChannel.CategoryId = CategoryId;
            return restChannel;
        }
    }
}
