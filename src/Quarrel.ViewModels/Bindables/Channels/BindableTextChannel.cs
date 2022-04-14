// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="GuildTextChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableTextChannel : BindableGuildChannel, IBindableMessageChannel
    {
        internal BindableTextChannel(GuildTextChannel channel, GuildMember selfMember, BindableCategoryChannel? parent = null) :
            base(channel, selfMember, parent)
        {
        }
        
        /// <inheritdoc/>
        public ulong Id => Channel.Id;

        /// <inheritdoc/>
        public override bool IsTextChannel => true;

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.ReadMessages;
        
        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;
    }
}
