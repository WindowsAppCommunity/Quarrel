// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="GuildTextChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableTextChannel : BindableGuildChannel
    {
        internal BindableTextChannel(GuildTextChannel channel, GuildMember selfMember, BindableCategoryChannel? parent = null) :
            base(channel, selfMember, parent)
        {
        }
        
        /// <inheritdoc/>
        public override bool IsTextChannel => true;

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.ReadMessages;
    }
}
