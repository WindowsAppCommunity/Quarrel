// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Users;
using Discord.API.Models.Json.Channels;
using System.Linq;

namespace Discord.API.Models.Channels
{
    internal class GroupChannel : Channel, IGroupChannel
    {
        public GroupChannel(JsonChannel restChannel, DiscordClient context) :
            base(restChannel, context)
        {
            Guard.IsNotNull(restChannel.OwnerId, nameof(restChannel.OwnerId));
            Guard.IsNotNull(restChannel.Recipients, nameof(restChannel.Recipients));

            OwnerId = restChannel.OwnerId.Value;

            RTCRegion = restChannel.RTCRegion;
            Recipients = restChannel.Recipients.Select(x => new User(x, context)).ToArray();
            LastMessageId = restChannel.LastMessageId;
        }

        public ulong OwnerId { get; private set; }

        public string? RTCRegion { get; private set; }

        public User[] Recipients { get; private set; }

        IUser[] IGroupChannel.Recipients => Recipients;

        public int? MentionCount { get; private set; }

        public ulong? LastMessageId { get; private set; }

        public ulong? LastReadMessageId { get; private set; }

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
            restChannel.OwnerId = OwnerId;
            restChannel.RTCRegion = RTCRegion;
            restChannel.Recipients = Recipients.Select(x => x.ToRestUser()).ToArray();
            return restChannel;
        }
    }
}
