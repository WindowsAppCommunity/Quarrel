// Adam Dernis © 2022

using Discord.API.Models;
using Discord.API.Models.Managed.Channels.Abstract;
using Discord.API.Models.Users;

namespace Quarrel.Bindables.Channels.Abstract
{
    public abstract class BindableGuildChannel : BindableChannel
    {
        internal BindableGuildChannel(GuildChannel channel, GuildMember selfMember) : base(channel)
        {
            Permissions = new Permissions();
            var roles = selfMember.GetRoles();
            foreach (var role in roles)
            {
                Permissions += role.Permissions;
            }

            ApplyOverrides(channel.PermissionOverwrites, selfMember);
        }

        public Permissions Permissions { get; private set; }

        private void ApplyOverrides(PermissionOverwrite[] overwrites, GuildMember selfMember)
        {
            var roles = selfMember.GetRoles();
            foreach (var overwrite in overwrites)
            {
                if (overwrite.Type == 0 && overwrite.Id == selfMember.GuildId || selfMember.HasRole(overwrite.Id))
                {
                    Permissions -= overwrite.Deny;
                    Permissions += overwrite.Allow;
                }
                else if (overwrite.Type == 1 && selfMember.UserId == overwrite.Id)
                {
                    Permissions -= overwrite.Deny;
                    Permissions += overwrite.Allow;
                }
            }
        }
    }
}
