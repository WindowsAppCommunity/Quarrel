// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Messages.Discord.Voice;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of an <see cref="IPrivateChannel"/> that can be bound to the UI.
    /// </summary>
    public abstract class BindablePrivateChannel : BindableChannel, IBindableMessageChannel, IBindableAudioChannel
    {
        private bool _isConnected;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindablePrivateChannel"/> class.
        /// </summary>
        internal BindablePrivateChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            PrivateChannel privateChannel) :
            base(messenger, clipboardService, discordService, dispatcherService, privateChannel)
        {
            SelectionCommand = new RelayCommand(Select);
            JoinCallCommand = new RelayCommand(JoinCall);
            MarkAsReadCommand = new RelayCommand(MarkRead);

            _messenger.Register<MyVoiceStateUpdatedMessage>(this, (_, m) =>
            {
                IsConnected = m.VoiceState.Channel?.Id == Id;
            });
        }


        /// <inheritdoc/>
        public override ulong? GuildId => null;

        /// <inheritdoc/>
        public override bool IsAccessible => true;

        /// <inheritdoc/>
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;

        /// <inheritdoc/>
        public IAudioChannel AudioChannel => (IAudioChannel)Channel;

        /// <inheritdoc/>
        public RelayCommand SelectionCommand { get; }

        /// <inheritdoc/>
        public RelayCommand JoinCallCommand { get; }

        /// <inheritdoc/>
        public RelayCommand MarkAsReadCommand { get; }

        /// <summary>
        /// Creates a new instance of a <see cref="BindablePrivateChannel"/> based on the type.
        /// </summary>
        /// <param name="messenger">The <see cref="IMessenger"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="clipboardService">The <see cref="IClipboardService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="discordService">The <see cref="IDiscordService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="localizationService">The <see cref="ILocalizationService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/> to pass to the <see cref="BindableItem"/>.</param>
        /// <param name="channel">The channel to wrap.</param>
        public static BindablePrivateChannel? Create(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            ILocalizationService localizationService,
            IDispatcherService dispatcherService,
            IPrivateChannel channel)
        {
            return BindableChannel.Create(messenger, clipboardService, discordService, localizationService, dispatcherService, channel) as BindablePrivateChannel;
        }

        /// <inheritdoc/>
        public void MarkRead()
            => _ = _discordService.MarkRead(MessageChannel.Id, MessageChannel.LastMessageId ?? 0);

        /// <inheritdoc/>
        public void Select() => _messenger.Send(new SelectChannelMessage<IBindableSelectableChannel>(this));

        /// <inheritdoc/>
        // TODO: Ring if there's no open call.
        public void JoinCall() => _ = _discordService.JoinCall(Id);

        /// <inheritdoc/>
        protected override void AckUpdate()
        {
            base.AckUpdate();
            OnPropertyChanged(nameof(MessageChannel));
        }
    }
}
