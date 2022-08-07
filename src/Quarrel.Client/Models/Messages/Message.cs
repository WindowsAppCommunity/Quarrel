// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models.Enums.Messages;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Messages.Embeds;
using Quarrel.Client.Models.Messages.Interfaces;
using Quarrel.Client.Models.Users;
using System;

// JSON models don't need to respect standard nullable rules.
#pragma warning disable CS8618

namespace Quarrel.Client.Models.Messages
{
    /// <summary>
    /// A message managed by a <see cref="QuarrelClient"/>.
    /// </summary>
    public class Message : SnowflakeItem, IMessage
    {
        internal Message(JsonMessage jsonMessage, QuarrelClient context) :
            base(context)
        {
            Id = jsonMessage.Id;
            ChannelId = jsonMessage.ChannelId;
            GuildId = jsonMessage.GuildId;
            Type = jsonMessage.Type;
            IsTextToSpeech = jsonMessage.IsTextToSpeech ?? false;
            IsPinned = jsonMessage.Pinned ?? false;
            MentionsEveryone = jsonMessage.MentionsEveryone;
            Timestamp = jsonMessage.Timestamp ?? DateTimeOffset.MinValue;
            EditedTimestamp = jsonMessage.EditedTimestamp;
            Content = jsonMessage.Content ?? string.Empty;
            Flags = jsonMessage.Flags;
            WebhookId = jsonMessage.WebhookId;

            Guard.IsNotNull(jsonMessage.Author, nameof(jsonMessage.Author));
            Author = context.Users.GetOrAddUser(jsonMessage.Author);

            if (jsonMessage.UserMentions is not null)
            {
                Mentions = new User[jsonMessage.UserMentions.Length];
                for (int i = 0; i < Mentions.Length; i++)
                {
                    Mentions[i] = context.Users.GetOrAddUser(jsonMessage.UserMentions[i]);
                }
            }
            else
            {
                Mentions = Array.Empty<User>();
            }

            if (jsonMessage.Attachments is not null)
            {
                Attachments = new Attachment[jsonMessage.Attachments.Length];
                for (int i = 0; i < Attachments.Length; i++)
                {
                    Attachments[i] = new Attachment(jsonMessage.Attachments[i], context);
                }
            }
            else
            {
                Attachments = Array.Empty<Attachment>();
            }

            if (jsonMessage.Reactions is not null)
            {
                Reactions = new Reaction[jsonMessage.Reactions.Length];
                for (int i = 0; i < Reactions.Length; i++)
                {
                    Reactions[i] = new Reaction(jsonMessage.Reactions[i], this, context);
                }
            }
            else
            {
                Reactions = Array.Empty<Reaction>();
            }

            // TODO: Create Interaction type
            Interaction = jsonMessage.Interaction;
        }

        /// <inheritdoc/>
        public ulong ChannelId { get; }

        /// <inheritdoc/>
        public ulong? GuildId { get; }

        /// <inheritdoc/>
        public MessageType Type { get; }

        /// <inheritdoc/>
        public bool IsTextToSpeech { get; }

        /// <inheritdoc/>
        public bool IsPinned { get; }

        /// <inheritdoc/>
        public bool MentionsEveryone { get; }

        /// <inheritdoc/>
        public string Content { get; }

        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; }

        /// <inheritdoc/>
        public DateTimeOffset? EditedTimestamp { get; }

        /// <inheritdoc/>
        public User Author { get; }

        /// <inheritdoc/>
        public bool IsOwn => Author.Id == Context.Self.CurrentUser?.Id;

        /// <inheritdoc/>
        public User[] Mentions { get; }

        /// <inheritdoc/>
        public Attachment[] Attachments { get; }
        
        /// <inheritdoc/>
        public Reaction[] Reactions { get; }

        /// <inheritdoc/>
        public MessageFlags? Flags { get; }

        /// <inheritdoc/>
        public object? Interaction { get; }

        /// <inheritdoc/>
        public ulong? WebhookId { get; }

        /// <inheritdoc/>
        public Uri MessageUri
            => new($"https://discord.com/channels/{GuildDisplayId}/{ChannelId}/{Id}");

        private string GuildDisplayId => GuildId.HasValue ? $"{GuildId.Value}" : "@me";
    }
}
