// Quarrel © 2022

using System;

namespace Quarrel.Client.Models.Users.Interfaces
{
    internal interface IGuildMember
    {
        /// <summary>
        /// Gets the time the user joined the guild.
        /// </summary>
        DateTimeOffset? JoinedAt { get; }

        /// <summary>
        /// Gets the member nickname in the guild.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// Gets the guild specific avatar for the user.
        /// </summary>
        string? GuildAvatarId { get; }

        /// <summary>
        /// Gets the ids of the user's roles.
        /// </summary>
        ulong[] Roles { get; }

        /// <summary>
        /// Gets the id of the user's top hoisted role.
        /// </summary>
        ulong? HoistedRole { get; }

        /// <summary>
        /// Gets the guild id of the guild the member is in.
        /// </summary>
        ulong GuildId { get; }

        /// <summary>
        /// Gets the user id of the member information.
        /// </summary>
        ulong UserId { get; }
    }
}
