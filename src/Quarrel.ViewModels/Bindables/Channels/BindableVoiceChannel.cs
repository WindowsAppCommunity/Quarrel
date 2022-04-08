// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Users;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableVoiceChannel : BindableGuildChannel
    {
        internal BindableVoiceChannel(VoiceChannel channel, GuildMember selfMember) : base(channel, selfMember)
        {
        }

        public override bool IsTextChannel => false;
    }
}
