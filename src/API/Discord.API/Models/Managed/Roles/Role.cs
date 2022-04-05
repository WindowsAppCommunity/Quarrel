// Adam Dernis © 2022

using Discord.API.Models.Base;
using Discord.API.Models.Enums.Permissions;
using Discord.API.Models.Json.Roles;
using Discord.API.Models.Roles.Interfaces;

namespace Discord.API.Models.Roles
{
    internal class Role : SnowflakeItem, IRole
    {
        internal Role(JsonRole jsonRole, DiscordClient context) :
            base(context)
        {
            Id = jsonRole.Id;
            Name = jsonRole.Name;
            Position = jsonRole.Position;
            Permissions = (Permission)jsonRole.Permissions;
            IsHoisted = jsonRole.Hoist;
            IsMangaged = jsonRole.Managed;
            IsMentionable = jsonRole.Mentionable;
        }

        public string Name { get; private set; }

        public string Icon { get; private set; }

        public int Position { get; private set; }

        public Permission Permissions { get; private set; }

        public bool IsHoisted { get; private set; }

        public bool IsMangaged { get; private set; }

        public bool IsMentionable { get; private set; }
    }
}
