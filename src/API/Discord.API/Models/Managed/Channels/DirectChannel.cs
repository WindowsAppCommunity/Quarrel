﻿// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Users;

namespace Discord.API.Models.Channels
{
    public class DirectChannel : Channel, IDirectChannel
    {
        internal DirectChannel(JsonChannel restChannel, DiscordClient context) : base(restChannel, context)
        {            
            Guard.IsNotNull(restChannel.Recipient, nameof(restChannel.Recipient));

            RecipientId = restChannel.Recipient.Id;
            LastMessageId = restChannel.LastMessageId;
        }

        public ulong RecipientId { get; private set; }

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

        public User GetRecipient(DiscordClient context)
        {
            User? user = context.GetUserInternal(RecipientId);
            Guard.IsNotNull(user, nameof(user));
            return user;
        }

        internal override void UpdateFromRestChannel(JsonChannel jsonChannel)
        {
            base.UpdateFromRestChannel(jsonChannel);

            if (jsonChannel.Recipient is not null)
            {
                Context.AddUser(jsonChannel.Recipient);
            }
        }

        internal override JsonChannel ToRestChannel()
        {
            JsonChannel restChannel = base.ToRestChannel();
            restChannel.Recipient = Context.GetUserInternal(RecipientId)?.ToRestUser();
            return restChannel;
        }
    }
}