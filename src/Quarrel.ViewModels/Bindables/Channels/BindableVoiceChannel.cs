// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableVoiceChannel : BindableChannel
    {
        internal BindableVoiceChannel(VoiceChannel channel) : base(channel)
        {
        }
        public override bool IsSelectable => false;
    }
}
