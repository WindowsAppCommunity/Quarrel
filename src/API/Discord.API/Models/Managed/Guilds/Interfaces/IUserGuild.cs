// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Permissions;

namespace Discord.API.Models.Guilds.Interfaces
{
    public interface IUserGuild : ISnowflakeItem
    {
        string Name { get; }

        string IconUrl { get; }

        bool IsOwner { get; }

        GuildPermission Permissions { get; }
    }
}
