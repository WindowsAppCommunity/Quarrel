// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableTextChannel : BindableGuildChannel
    {
        internal BindableTextChannel(GuildTextChannel channel, GuildMember selfMember) :
            base(channel, selfMember)
        {
        }

        public override bool IsTextChannel => true;
    }
}
