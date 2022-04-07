// Adam Dernis © 2022

using Discord.API.Models.Channels.Abstract;
using Quarrel.Bindables.Channels.Abstract;

namespace Quarrel.Bindables.Channels
{
    public class BindableTextChannel : BindableChannel
    {
        internal BindableTextChannel(Channel channel) : base(channel)
        {
        }
    }
}
