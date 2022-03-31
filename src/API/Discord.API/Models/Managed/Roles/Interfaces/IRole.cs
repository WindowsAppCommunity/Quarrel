// Adam Dernis © 2022

using Discord.API.Models.Base.Interfaces;
using Discord.API.Models.Enums.Permissions;

namespace Discord.API.Models.Roles.Interfaces
{
    public interface IRole : ISnowflakeItem
    {
        string Name { get; }

        string Icon { get; }

        int Position { get; }

        GuildPermission Permissions { get; }

        bool IsHoisted { get; }

        bool IsMangaged { get; }

        bool IsMentionable { get; }
    }
}
