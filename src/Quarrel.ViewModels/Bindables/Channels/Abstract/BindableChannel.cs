// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Messages.Discord;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;

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
        internal BindableChannel(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Channel channel) :
            base(messenger, discordService, dispatcherService)
        {
            _channel = channel;

            messenger.Register<ChannelUpdatedMessage>(this, (_, e) =>
            {
                if (Id == e.Channel.Id)
                {
                    Channel = e.Channel;
                }
            });
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
                SetProperty(ref _channel, value);
                AckUpdateRoot();
            }
        }

        /// <inheritdoc/>
        public abstract ulong? GuildId { get; }

        /// <inheritdoc/>
        public abstract bool IsAccessible { get; }

        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Channel));
            OnPropertyChanged(nameof(Name));
        }

        private void AckUpdateRoot()
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
        public static BindableChannel? Create(
            IMessenger messenger,
            IDiscordService discordService,
            ILocalizationService localizationService,
            IDispatcherService dispatcherService,
            IChannel channel,
            GuildMember? member = null,
            BindableCategoryChannel? parent = null)
        {
            if (member is null)
            {
                return channel switch
                {
                    DirectChannel c => new BindableDirectChannel(messenger, discordService, dispatcherService, c),
                    GroupChannel c => new BindableGroupChannel(messenger, discordService, localizationService, dispatcherService, c),
                    _ => null
                };
            }

            return channel switch
            {
                GuildTextChannel c => new BindableTextChannel(messenger, discordService, dispatcherService, c, member, parent),
                VoiceChannel c => new BindableVoiceChannel(messenger, discordService, dispatcherService, c, member, parent),
                CategoryChannel c => new BindableCategoryChannel(messenger, discordService, dispatcherService, c, member),
                _ => null
            };
        }
    }
}
