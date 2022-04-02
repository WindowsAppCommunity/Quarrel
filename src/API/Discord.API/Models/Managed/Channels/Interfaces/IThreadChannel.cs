// Adam Dernis © 2022

using Discord.API.Models.Enums.Channels;
using System;

namespace Discord.API.Models.Channels.Interfaces
{
    internal interface IThreadChannel : IGuildTextChannel
    {
        ThreadType ThreadType { get; }

        bool HasJoined { get; }

        bool IsArchived { get; }

        ThreadArchiveDuration AutoArchiveDuration { get; }

        DateTimeOffset ArchiveTimestamp { get; }

        DateTimeOffset CreatedAt { get; }

        bool IsLocked { get; }

        int MemberCount { get; }

        int MessageCount { get; }

        bool? IsInvitable { get; }
    }
}
