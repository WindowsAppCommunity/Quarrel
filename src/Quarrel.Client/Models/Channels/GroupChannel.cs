// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Client.Models.Users.Interfaces;
using System.Linq;

namespace Quarrel.Client.Models.Channels
{
    /// <summary>
    /// A group dm channel managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class GroupChannel : Channel, IGroupChannel
    {
        internal GroupChannel(JsonChannel restChannel, QuarrelClient context) :
            base(restChannel, context)
        {
            Guard.IsNotNull(restChannel.OwnerId, nameof(restChannel.OwnerId));
            Guard.IsNotNull(restChannel.Recipients, nameof(restChannel.Recipients));

            OwnerId = restChannel.OwnerId.Value;

            RTCRegion = restChannel.RTCRegion;
            Recipients = restChannel.Recipients.Select(x => new User(x, context)).ToArray();
            LastMessageId = restChannel.LastMessageId;
        }

        /// <inheritdoc/>
        public ulong OwnerId { get; private set; }

        /// <inheritdoc/>
        public string? RTCRegion { get; private set; }

        /// <inheritdoc/>
        public User[] Recipients { get; private set; }

        IUser[] IGroupChannel.Recipients => Recipients;

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

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            restChannel.OwnerId = OwnerId;
            restChannel.RTCRegion = RTCRegion;
            restChannel.Recipients = Recipients.Select(x => x.ToRestUser()).ToArray();
            return restChannel;
        }
    }
}
