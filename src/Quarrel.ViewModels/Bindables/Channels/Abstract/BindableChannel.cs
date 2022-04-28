// Quarrel © 2022

using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using System;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.Abstract.Channel"/> that can be bound to the UI.
    /// </summary>
    public abstract partial class BindableChannel : SelectableItem, IBindableChannel
    {
        private Channel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableChannel"/> class.
        /// </summary>
        internal BindableChannel(IDiscordService discordService, IDispatcherService dispatcherService, Channel channel) :
            base(discordService, dispatcherService)
        {
            _channel = channel;
            _channel.ItemUpdated += AckUpdateRoot;
        }

        /// <inheritdoc/>
        public ulong Id => Channel.Id;

        /// <inheritdoc/>
        public virtual string? Name => _channel.Name;

        /// <summary>
        /// Gets a bool representing whether or not the channel is a text channel.
        /// </summary>
        public virtual bool IsTextChannel => true;

        /// <summary>
        /// Gets the wrapped <see cref="Client.Models.Channels.Abstract.Channel"/>.
        /// </summary>
        public Channel Channel
        {
            get => _channel;
            private set
            {
                if (_channel is not null)
                {
                    _channel.ItemUpdated -= AckUpdateRoot;
                }

                if (SetProperty(ref _channel, value))
                {
                    OnPropertyChanged(nameof(Name));
                    _channel.ItemUpdated += AckUpdateRoot;
                }
            }
        }

        /// <inheritdoc/>
        public abstract ulong? GuildId { get; }

        /// <inheritdoc/>
        public abstract bool IsAccessible { get; }

        /// <summary>
        /// Invokes property changed for mutable properties when <see cref="Channel.ItemUpdated"/> is invoked.
        /// </summary>
        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Channel));
            OnPropertyChanged(nameof(Name));
        }

        private void AckUpdateRoot(object sender, EventArgs e)
        {
            _dispatcherService.RunOnUIThread(() =>
            {
                AckUpdate();
            });
        }

        /// <summary>
        /// Creates a new instance of a <see cref="BindableChannel"/> based on the type.
        /// </summary>
        /// <param name="discordService">The <see cref="IDiscordService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="localizationService">The <see cref="ILocalizationService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="channel">The channel to wrap.</param>
        /// <param name="member">The current user's guild member for the channel's guild. Null if not a guild channel.</param>
        /// <param name="parent">The parent category of the channel.</param>
        public static BindableChannel? Create(IDiscordService discordService, ILocalizationService localizationService, IDispatcherService dispatcherService, IChannel channel, GuildMember? member = null, BindableCategoryChannel? parent = null)
        {
            if (member is null)
            {
                return channel switch
                {
                    DirectChannel c => new BindableDirectChannel(discordService, dispatcherService, c),
                    GroupChannel c => new BindableGroupChannel(discordService, localizationService, dispatcherService, c),
                    _ => null
                };
            }

            return channel switch
            {
                GuildTextChannel c => new BindableTextChannel(discordService, dispatcherService, c, member, parent),
                VoiceChannel c => new BindableVoiceChannel(discordService, dispatcherService, c, member, parent),
                CategoryChannel c => new BindableCategoryChannel(discordService, dispatcherService, c, member),
                _ => null
            };
        }
    }
}
