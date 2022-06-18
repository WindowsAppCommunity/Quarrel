// Quarrel © 2022

using Discord.API.Models.Enums.Channels;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Messages.Discord.Channels;
using Quarrel.Messages.Discord.Messages;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.Abstract.Channel"/> that can be bound to the UI.
    /// </summary>
    public abstract class BindableChannel : SelectableItem, IBindableChannel
    {
        /// <summary>
        /// The <see cref="IClipboardService"/> service.
        /// </summary>
        protected readonly IClipboardService _clipboardService;
        private Channel _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableChannel"/> class.
        /// </summary>
        internal BindableChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Channel channel) :
            base(messenger, discordService, dispatcherService)
        {
            _clipboardService = clipboardService;
            _channel = channel;

            _messenger.Register<ChannelUpdatedMessage>(this, (_, m) =>
            {
                if (Id == m.Channel.Id)
                {
                    Channel = m.Channel;
                }
            });
            _messenger.Register<MessageMarkedReadMessage>(this, (_, m) =>
            {
                if (m.ChannelId != Channel.Id) return;
                AckUpdateRoot();
            });

            CopyIdCommand = new RelayCommand(() => _clipboardService.Copy($"{Id}"));
            CopyLinkCommand = new RelayCommand(() => _clipboardService.Copy(Channel.Url));
        }

        /// <inheritdoc/>
        public ulong Id => Channel.Id;

        /// <inheritdoc/>
        public ChannelType Type => Channel.Type;

        /// <inheritdoc/>
        public virtual string? Name => _channel.Name;

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

        /// <inheritdoc/>
        public RelayCommand CopyLinkCommand { get; }

        /// <inheritdoc/>
        public RelayCommand CopyIdCommand { get; }

        /// <summary>
        /// A virtual method that notifies property changed for a <see cref="BindableChannel"/>.
        /// </summary>
        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Channel));
            OnPropertyChanged(nameof(Name));
        }

        private void AckUpdateRoot()
        {
            _dispatcherService.RunOnUIThread(AckUpdate);
        }

        /// <summary>
        /// Creates a new instance of a <see cref="BindableChannel"/> based on the type.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="clipboardService">The <see cref="IClipboardService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="discordService">The <see cref="IDiscordService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="localizationService">The <see cref="ILocalizationService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="channel">The channel to wrap.</param>
        /// <param name="member">The current user's guild member for the channel's guild. Null if not a guild channel.</param>
        /// <param name="parent">The parent category of the channel.</param>
        public static BindableChannel? Create(
            IMessenger messenger,
            IClipboardService clipboardService,
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
                    DirectChannel c => new BindableDirectChannel(messenger, clipboardService, discordService, dispatcherService, c),
                    GroupChannel c => new BindableGroupChannel(messenger, clipboardService, discordService, localizationService, dispatcherService, c),
                    _ => null
                };
            }

            return channel switch
            {
                GuildTextChannel c => new BindableTextChannel(messenger, clipboardService, discordService, dispatcherService, c, member, parent),
                VoiceChannel c => new BindableVoiceChannel(messenger, clipboardService, discordService, dispatcherService, c, member, parent),
                CategoryChannel c => new BindableCategoryChannel(messenger, clipboardService, discordService, dispatcherService, c, member),
                _ => null
            };
        }
    }
}
