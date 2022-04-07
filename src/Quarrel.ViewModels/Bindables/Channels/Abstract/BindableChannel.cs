// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

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
            Children = new ObservableCollection<BindableChannel>();
        }

        public virtual string? Name => _channel.Name;

        public ObservableCollection<BindableChannel> Children { get; }

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
