// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Users;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;

namespace Quarrel.Client.Models.Channels
{
    /// <summary>
    /// A direct message channel managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class DirectChannel : PrivateChannel, IDirectChannel
    {
        internal DirectChannel(JsonChannel restChannel, QuarrelClient context) :
            base(restChannel, context)
        {
            Guard.IsNotNull(restChannel.Recipients, nameof(restChannel.Recipients));

            RecipientId = restChannel.Recipients[0].Id;
            LastMessageId = restChannel.LastMessageId;
            RTCRegion = restChannel.RTCRegion;
        }

        /// <inheritdoc/>
        public ulong RecipientId { get; private set; }

        /// <inheritdoc/>
        public int? MentionCount { get; private set; }

        /// <inheritdoc/>
        public ulong? LastMessageId { get; private set; }

        /// <inheritdoc/>
        public ulong? LastReadMessageId { get; private set; }

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

        /// <inheritdoc/>
        public string? RTCRegion { get; private set; }

        /// <summary>
        /// Gets the recipient of the direct message channel.
        /// </summary>
        /// <returns>The recipient of the channel.</returns>
        public User GetRecipient()
        {
            User? user = Context.GetUserInternal(RecipientId);
            Guard.IsNotNull(user, nameof(user));
            return user;
        }

        internal override void PrivateUpdateFromJsonChannel(JsonChannel jsonChannel)
        {
            base.PrivateUpdateFromJsonChannel(jsonChannel);

            if (jsonChannel.Recipients is not null)
            {
                Context.AddUser(jsonChannel.Recipients[0]);
            }
        }

        internal override JsonChannel ToJsonChannel()
        {
            JsonChannel restChannel = base.ToJsonChannel();
            var recipient = Context.GetUserInternal(RecipientId)?.ToRestUser();
            if (recipient is not null)
            {
                restChannel.Recipients = new JsonUser[] { recipient };
            }

            return restChannel;
        }
    }
}
