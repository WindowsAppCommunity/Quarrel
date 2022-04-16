// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Models;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Permissions;
using Quarrel.Client.Models.Users;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of an <see cref="IGuildChannel"/> that can be bound to the UI.
    /// </summary>
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

            Guard.IsNotNull(channel.PermissionOverwrites, nameof(channel.PermissionOverwrites));
            ApplyOverrides(channel.PermissionOverwrites, selfMember);
        }

        /// <summary>
        /// The category the channel belongs to.
        /// </summary>
        public BindableCategoryChannel? CategoryChannel { get; }

        /// <summary>
        /// The permissions the user has in the channel.
        /// </summary>
        public Permissions Permissions { get; private set; }

        /// <summary>
        /// Gets if the user has permission to open the channel.
        /// </summary>
        public abstract bool IsAccessible { get; }

        /// <summary>
        /// Creates a new <see cref="BindableGuildChannel"/> based on the type.
        /// </summary>
        /// <param name="channel">The channel to wrap.</param>
        /// <param name="member">The current user's guild member for the channel's guild.</param>
        /// <param name="parent">The channel's parent category.</param>
        public static BindableGuildChannel? Create(IGuildChannel channel, GuildMember member, BindableCategoryChannel? parent = null)
        {
            return BindableChannel.Create(channel, member, parent) as BindableGuildChannel;
        }

        private void ApplyOverrides(PermissionOverwrite[] overwrites, GuildMember selfMember)
        {
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
