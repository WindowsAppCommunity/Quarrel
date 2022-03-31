// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;

namespace Discord.API.Models.Channels
{
    internal class GuildTextChannel : Channel, IGuildTextChannel
    {
        public GuildTextChannel(JsonChannel restChannel) : base(restChannel)
        {
            Guard.IsNotNull(restChannel.SlowModeDelay, nameof(restChannel.SlowModeDelay));
            Guard.IsNotNull(restChannel.CategoryId, nameof(restChannel.CategoryId));
            Guard.IsNotNull(restChannel.Position, nameof(restChannel.Position));
            Guard.IsNotNull(restChannel.GuildId, nameof(restChannel.GuildId));

            Topic = restChannel.Topic;
            IsNSFW = restChannel.IsNSFW;
            SlowModeDelay = restChannel.SlowModeDelay.Value;
            LastMessageId = restChannel.LastMessageId;
            CategoryId = restChannel.CategoryId.Value;
            Position = restChannel.Position.Value;
            GuildId = restChannel.GuildId.Value;
        }

        public string? Topic { get; private set; }

        public bool? IsNSFW { get; private set; }

        public int SlowModeDelay { get; private set; }

        public ulong? LastMessageId { get; private set; }

        public ulong CategoryId { get; private set; }

        public int Position { get; private set; }

        public ulong GuildId { get; private set; }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Topic = Topic;
            restChannel.IsNSFW = IsNSFW;
            restChannel.SlowModeDelay = SlowModeDelay;
            restChannel.LastMessageId = LastMessageId;
            restChannel.CategoryId = CategoryId;
            restChannel.Position = Position;
            restChannel.GuildId = GuildId;
            return restChannel;
        }
    }
}
