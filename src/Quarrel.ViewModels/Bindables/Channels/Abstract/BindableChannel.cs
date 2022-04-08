// Adam Dernis © 2022

using Discord.API.Models.Channels;
using Discord.API.Models.Channels.Abstract;
using Discord.API.Models.Channels.Interfaces;
using Discord.API.Models.Users;
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
        
        public abstract bool IsTextChannel { get; }

        public static BindableChannel? Create(IChannel channel, GuildMember member)
        {
            return channel switch
            {
                GuildTextChannel c=> new BindableTextChannel(c, member),
                VoiceChannel c => new BindableVoiceChannel(c, member),
                CategoryChannel c => new BindableCategoryChannel(c, member),
                _ => null
            };
        }
    }
}
