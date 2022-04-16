// Quarrel © 2022

using Discord.API.Models.Enums.Permissions;
using Quarrel.Client.Models.Base.Interfaces;

namespace Quarrel.Client.Models.Guilds.Interfaces
{
    internal interface IUserGuild : ISnowflakeItem
    {
        string Name { get; }

        string IconUrl { get; }

        bool IsOwner { get; }

        Permission Permissions { get; }
    }
}
