// Adam Dernis © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Enums.Messages;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Messages.Interfaces;
using System;

namespace Discord.API.Models.Messages
{
    public class Message : SnowflakeItem, IMessage
    {
        internal Message(JsonMessage jsonMessage, DiscordClient context) :
            base(context)
        {
            Type = jsonMessage.Type;
            IsTextToSpeech = jsonMessage.IsTextToSpeech ?? false;
            IsPinned = jsonMessage.Pinned ?? false;
            MentionsEveryone = jsonMessage.MentionsEveryone;
            Timestamp = jsonMessage.Timestamp ?? DateTimeOffset.MinValue;
            EditedTimestamp = jsonMessage.EditedTimestamp;
            Content = jsonMessage.Content ?? String.Empty;
        }

        public MessageType Type { get; private set; }

        public bool IsTextToSpeech { get; private set; }

        public bool IsPinned { get; private set; }

        public bool MentionsEveryone { get; private set; }

        public string Content { get; private set; }

        public DateTimeOffset Timestamp { get; private set; }

        public DateTimeOffset? EditedTimestamp { get; private set; }
    }
}
