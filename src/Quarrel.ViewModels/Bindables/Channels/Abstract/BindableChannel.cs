// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Quarrel.Bindables.Channels.Abstract
{
    public abstract partial class BindableChannel : ObservableObject
    {
        [AlsoNotifyChangeFor(nameof(Name))]
        [ObservableProperty]
        private Channel _channel;

        internal BindableChannel(Channel channel)
        {
            Channel = channel;
        }

        public virtual string? Name => _channel.Name;

        public static BindableChannel? Create(IChannel channel)
        {
            return channel switch
            {
                GuildTextChannel c=> new BindableTextChannel(c),
                CategoryChannel c => new BindableCategoryChannel(c),
                _ => null
            };
        }
    }
}
