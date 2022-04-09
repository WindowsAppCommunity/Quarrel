// Adam Dernis © 2022

using Discord.API.Models;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Managed.Channels.Abstract;
using Discord.API.Models.Users;

namespace Quarrel.Bindables.Channels.Abstract
{
    public abstract class BindableGuildChannel : BindableChannel
    {
        internal BindableGuildChannel(GuildChannel channel, GuildMember selfMember, BindableCategoryChannel? parent = null) : base(channel)
        {
            CategoryChannel = parent;

            if (CategoryChannel is null)
            {
                Permissions = new Permissions();
                var roles = selfMember.GetRoles();
                foreach (var role in roles)
                {
                    Permissions += role.Permissions;
                }
            }
            else
            {
                Permissions = CategoryChannel.Permissions;
            }

            ApplyOverrides(channel.PermissionOverwrites, selfMember);
        }


        public BindableCategoryChannel? CategoryChannel { get; }

        public Permissions Permissions { get; private set; }

        /// <summary>
        /// Gets if the user has permission to open the channel.
        /// </summary>
        public abstract bool IsAccessible { get; }

        public static BindableGuildChannel? Create(IGuildChannel channel, GuildMember member, BindableCategoryChannel? parent = null)
        {
            return BindableChannel.Create(channel, member, parent) as BindableGuildChannel;
        }

        private void ApplyOverrides(PermissionOverwrite[] overwrites, GuildMember selfMember)
        {
            var roles = selfMember.GetRoles();
            foreach (var overwrite in overwrites)
            {
                if (overwrite.Type == 0 && selfMember.HasRole(overwrite.Id))
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
