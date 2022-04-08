// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableCategoryChannel : BindableGuildChannel
    {
        internal BindableCategoryChannel(CategoryChannel channel, GuildMember selfMember) :
            base(channel, selfMember)
        {
        }

        public override bool IsTextChannel => false;

        /// <inheritdoc/>
        public override bool IsAccessible => true;
    }
}
