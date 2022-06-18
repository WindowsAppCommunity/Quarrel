// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.VoiceChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableVoiceChannel : BindableGuildChannel, IBindableAudioChannel, IBindableMessageChannel
    {
        internal BindableVoiceChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            VoiceChannel channel,
            GuildMember selfMember,
            BindableCategoryChannel? parent = null) :
            base(messenger, clipboardService, discordService, dispatcherService, channel, selfMember, parent)
        {
            SelectionCommand = new RelayCommand(Select);
            OpenChatCommand = new RelayCommand(OpenChat);
            JoinCallCommand = new RelayCommand(JoinCall);
            MarkAsReadCommand = new RelayCommand(MarkRead);
        }

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.Connect;

        /// <inheritdoc/>
        public IAudioChannel AudioChannel => (IAudioChannel)Channel;

        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;

        /// <inheritdoc/>
        public VoiceChannel VoiceChannel => (VoiceChannel)Channel;

        /// <inheritdoc/>
        public RelayCommand SelectionCommand { get; }

        /// <summary>
        /// Gets a command that opens the voice channel as a text chat.
        /// </summary>
        public RelayCommand OpenChatCommand { get; }

        /// <inheritdoc/>
        public RelayCommand JoinCallCommand { get; }

        /// <inheritdoc/>
        public RelayCommand MarkAsReadCommand { get; }

        /// <inheritdoc/>
        public void Select() => JoinCall();

        /// <summary>
        /// Opens the voice channel as a text chat.
        /// </summary>
        public void OpenChat()
        {
            _messenger.Send(new SelectChannelMessage<IBindableSelectableChannel>(this));
        }

        /// <inheritdoc/>
        public void JoinCall()
        {
            _ = _discordService.JoinCall(Id, GuildId);
        }

        /// <inheritdoc/>
        public void MarkRead()
        {
            _ = _discordService.MarkRead(MessageChannel.Id, MessageChannel.LastMessageId ?? 0);
        }
    }
}
