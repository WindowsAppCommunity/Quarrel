// Quarrel © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Permissions;

namespace Discord.API.Models.Guilds.Interfaces
{
    internal interface IUserGuild : ISnowflakeItem
    {
        string Name { get; }

        string IconUrl { get; }

        bool IsOwner { get; }

        Permission Permissions { get; }
    }
}
