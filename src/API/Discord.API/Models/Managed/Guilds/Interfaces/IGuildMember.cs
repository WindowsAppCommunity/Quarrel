// Adam Dernis © 2022

using System;

namespace Discord.API.Models.Guilds.Interfaces
{
    public interface IGuildMember
    {
        DateTimeOffset? JoinedAt { get; }

        string Nickname { get; }

        string? GuildAvatarId { get; }

        ulong[] Roles { get; }

        ulong? HoistedRole { get; }

        ulong GuildId { get; }

        ulong UserId { get; }
    }
}
