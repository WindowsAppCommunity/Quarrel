// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Base;
using Quarrel.Client.Models.Messages.Interfaces;
using Quarrel.Client.Models.Users;
using System;

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
            Type = jsonMessage.Type;
            IsTextToSpeech = jsonMessage.IsTextToSpeech ?? false;
            IsPinned = jsonMessage.Pinned ?? false;
            MentionsEveryone = jsonMessage.MentionsEveryone;
            Timestamp = jsonMessage.Timestamp ?? DateTimeOffset.MinValue;
            EditedTimestamp = jsonMessage.EditedTimestamp;
            Content = jsonMessage.Content ?? string.Empty;

            Author = context.GetOrAddUserInternal(jsonMessage.Author);
        }

        /// <inheritdoc/>
        public MessageType Type { get; private set; }

        /// <inheritdoc/>
        public bool IsTextToSpeech { get; private set; }

        /// <inheritdoc/>
        public bool IsPinned { get; private set; }

        /// <inheritdoc/>
        public bool MentionsEveryone { get; private set; }

        /// <inheritdoc/>
        public string Content { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset Timestamp { get; private set; }

        /// <inheritdoc/>
        public DateTimeOffset? EditedTimestamp { get; private set; }

        /// <inheritdoc/>
        public User Author { get; private set; }
    }
}
