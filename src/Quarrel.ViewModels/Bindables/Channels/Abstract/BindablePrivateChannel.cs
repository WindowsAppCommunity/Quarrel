// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;

namespace Quarrel.Bindables.Channels.Abstract
{
    /// <summary>
    /// A wrapper of an <see cref="IPrivateChannel"/> that can be bound to the UI.
    /// </summary>
    public abstract class BindablePrivateChannel : BindableChannel, IBindableMessageChannel
    {
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
            StartCallCommand = new RelayCommand(() => _discordService.StartCall(Id));
            MarkAsReadCommand = new RelayCommand(() => _discordService.MarkRead(Id, MessageChannel.LastMessageId ?? 0));
        }

        /// <inheritdoc/>
        public override ulong? GuildId => null;

        /// <inheritdoc/>
        public override bool IsAccessible => true;

        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;

        /// <summary>
        /// Gets a command that begins a call.
        /// </summary>
        public RelayCommand StartCallCommand { get; }

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
    }
}
