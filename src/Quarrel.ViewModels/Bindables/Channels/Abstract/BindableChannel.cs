// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;

namespace Quarrel.Bindables.Channels.Abstract
{
    public abstract partial class BindableChannel : SelectableItem
    {
        [AlsoNotifyChangeFor(nameof(Name))]
        [ObservableProperty]
        private Channel _channel;

        internal BindableChannel(Channel channel)
        {
            Channel = channel;
        }

        public virtual string? Name => _channel.Name;

        public virtual bool IsSelectable => true;

        public static BindableChannel? Create(IChannel channel)
        {
            return channel switch
            {
                GuildTextChannel c=> new BindableTextChannel(c),
                VoiceChannel c => new BindableVoiceChannel(c),
                CategoryChannel c => new BindableCategoryChannel(c),
                _ => null
            };
        }
    }
}
