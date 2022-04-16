// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Quarrel.Client.Models.Base.Interfaces;
using System;

namespace Quarrel.Client.Models.Messages.Interfaces
{
    internal interface IMessage : ISnowflakeItem
    {
        /// <summary>
        /// Gets the type of message.
        /// </summary>
        MessageType Type { get; }

        /// <summary>
        /// Gets whether or not the message should be read out-load.
        /// </summary>
        bool IsTextToSpeech { get; }

        /// <summary>
        /// Gets whether or not the message is pinned.
        /// </summary>
        bool IsPinned { get; }

        /// <summary>
        /// Gets whether or not the message mentions @everyone.
        /// </summary>
        bool MentionsEveryone { get; }

        /// <summary>
        /// Gets the markdown content of the message.
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Gets the timestamp the message was created.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the timestamp the message was last edited.
        /// </summary>
        DateTimeOffset? EditedTimestamp { get; }
    }
}
