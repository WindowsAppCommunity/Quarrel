// Quarrel © 2022

using Quarrel.Bindables.Abstract;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.Abstract.Channel"/> that can be bound to the UI.
    /// </summary>
    public abstract partial class BindableChannel : SelectableItem
    {
        private Channel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableChannel"/> class.
        /// </summary>
        /// <param name="channel">The <see cref="Client.Models.Channels.Abstract.Channel"/> to wrap.</param>
        internal BindableChannel(Channel channel)
        {
            _channel = channel;
        }

        /// <summary>
        /// Gets the name of the channel as displayed.
        /// </summary>
        public virtual string? Name => _channel.Name;

        /// <summary>
        /// Gets a bool representing whether or not the channel is a text channel.
        /// </summary>
        public abstract bool IsTextChannel { get; }

        /// <summary>
        /// Gets the wrapped <see cref="Client.Models.Channels.Abstract.Channel"/>.
        /// </summary>
        public Channel Channel
        {
            get => _channel;
            private set
            {
                if (SetProperty(ref _channel, value))
                {
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a <see cref="BindableChannel"/> based on the type.
        /// </summary>
        /// <param name="channel">The channel to wrap.</param>
        /// <param name="member">The current user's guild member for the channel's guild. Null if not a guild channel.</param>
        /// <param name="parent">The parent category of the channel.</param>
        public static BindableChannel? Create(IChannel channel, GuildMember member, BindableCategoryChannel? parent = null)
        {
            return channel switch
            {
                GuildTextChannel c => new BindableTextChannel(c, member, parent),
                VoiceChannel c => new BindableVoiceChannel(c, member, parent),
                CategoryChannel c => new BindableCategoryChannel(c, member),
                _ => null
            };
        }
    }
}
