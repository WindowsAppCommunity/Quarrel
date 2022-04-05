// Adam Dernis © 2022

using Discord.API.Models.Enums.Channels;
using System;

namespace Discord.API.Models.Channels.Interfaces
{
    /// <summary>
    /// An interface for threads.
    /// </summary>
    internal interface IThreadChannel : IGuildTextChannel
    {
        /// <summary>
        /// The type of thread.
        /// </summary>
        ThreadType ThreadType { get; }

        /// <summary>
        /// Gets whether or not the current user has joined the thread.
        /// </summary>
        bool HasJoined { get; }

        /// <summary>
        /// Gets whether or not the thread is archived.
        /// </summary>
        bool IsArchived { get; }

        /// <summary>
        /// Gets the duration of inactivity until the thread auto archives.
        /// </summary>
        ThreadArchiveDuration AutoArchiveDuration { get; }

        /// <summary>
        /// Gets the time the thread was or will be archived.
        /// </summary>
        DateTimeOffset ArchiveTimestamp { get; }

        /// <summary>
        /// Gets the time the thread was created at.
        /// </summary>
        DateTimeOffset CreatedAt { get; }

        /// <summary>
        /// Gets whether or not the thread is locked.
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// Gets the number of members in the thread.
        /// </summary>
        int MemberCount { get; }

        /// <summary>
        /// Gets the number of messages in the thread.
        /// </summary>
        int MessageCount { get; }

        /// <summary>
        /// Gets whether or not users can be invited to the thread.
        /// </summary>
        bool? IsInvitable { get; }
    }
}
