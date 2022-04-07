// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace Quarrel.Bindables.Channels.Abstract
{
    public abstract partial class BindableChannel : ObservableObject
    {
        [AlsoNotifyChangeFor(nameof(Name))]
        [ObservableProperty]
        private Channel _channel;

        public BindableChannel(Channel channel)
        {
            Channel = channel;
        }

        public virtual string? Name => _channel.Name;

        public static BindableChannel? Create(Channel channel)
        {
            return channel switch
            {
                GuildTextChannel guildTextChannel => new BindableTextChannel(guildTextChannel),
                _ => null
            };
        }
    }
}
