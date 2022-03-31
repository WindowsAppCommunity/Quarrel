// Adam Dernis © 2022

using Discord.API.Models.Users;
using System;

namespace Discord.API.Models.Guilds.Interfaces
{
    public interface IGuildMember
    {
        IUser User { get; }

        DateTimeOffset? JoinedAt { get; }

        string Nickname { get; }

        string GuildAvatarId { get; }

        ulong GuildId { get; }
    }
}
