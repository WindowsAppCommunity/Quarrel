// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Messages;
using System;

namespace Discord.API.Models.Messages.Interfaces
{
    public interface IMessage : ISnowflakeItem
    {
        MessageType Type { get; }

        bool IsTextToSpeech { get; }

        bool IsPinned { get; }

        bool MentionsEveryone { get; }

        string Content { get; }

        DateTimeOffset Timestamp { get; }

        DateTimeOffset? EditedTimestamp { get; }
    }
}
