// Adam Dernis © 2022

using Discord.API.Models.Enums.Permissions;
using Discord.API.Models.Json.Permissions;

namespace Discord.API.Models
{
    public class PermissionOverwrite
    {
        internal PermissionOverwrite(JsonOverwrite jsonOverwrite)
        {
            Id = jsonOverwrite.Id;
            Type = jsonOverwrite.Type;
            Allow = (Permission)jsonOverwrite.Allow;
            Deny = (Permission)jsonOverwrite.Deny;
        }

        public ulong Id { get; }

        public int Type { get; }

        public Permissions Allow { get; }

        public Permissions Deny { get; }
    }
}
