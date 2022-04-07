// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableTextChannel : BindableChannel
    {
        internal BindableTextChannel(GuildTextChannel channel) : base(channel)
        {
        }
    }
}
