// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Enums;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Voice;
using Quarrel.Client;
using Quarrel.Client.Models.Channels;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Users;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Messages.Discord.Voice;
using System.Collections.ObjectModel;
using System.Linq;

namespace Quarrel.Bindables.Channels
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Channels.VoiceChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableVoiceChannel : BindableGuildChannel, IBindableAudioChannel, IBindableMessageChannel
    {
        private bool _isConnected;

        internal BindableVoiceChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            VoiceChannel channel,
            GuildMember selfMember,
            BindableCategoryChannel? parent = null) :
            base(messenger, clipboardService, discordService, quarrelClient, dispatcherService, channel, selfMember, parent)
        {
            SelectionCommand = new RelayCommand(Select);
            OpenChatCommand = new RelayCommand(OpenChat);
            JoinCallCommand = new RelayCommand(JoinCall);
            MarkAsReadCommand = new RelayCommand(MarkRead);

            VoiceMembers = new ObservableCollection<BindableVoiceState>();

            _messenger.Register<MyVoiceStateUpdatedMessage>(this, (_, m) =>
            {
                IsConnected = m.VoiceState.Channel?.Id == Id;
            });
            _messenger.Register<VoiceStateAddedMessage>(this, (_, m) =>
            {
                if (m.VoiceState.Channel?.Id == Id)
                {
                    _dispatcherService.RunOnUIThread(() =>
                    {
                        var state = new BindableVoiceState(
                            _messenger,
                            _discordService,
                            _quarrelClient,
                            _dispatcherService,
                            m.VoiceState);

                        VoiceMembers.Add(state);
                    });
                }
            });
            _messenger.Register<VoiceStateRemovedMessage>(this, (_, m) =>
            {
                if (m.VoiceState.Channel?.Id == Id)
                {
                    _dispatcherService.RunOnUIThread(() =>
                    {
                        VoiceMembers.Remove(VoiceMembers.FirstOrDefault(x =>
                            x.State.User?.Id == m.VoiceState.User?.Id));
                    });
                }
            });
        }

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.Connect;

        /// <inheritdoc/>
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

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
        public ObservableCollection<BindableVoiceState> VoiceMembers { get; }

        /// <inheritdoc/>
        public RelayCommand MarkAsReadCommand { get; }
        
        /// <inheritdoc/>
        public ReadState ReadState
        {
            get
            {
                // TODO: Handle muted.
                if (MessageChannel.IsUnread) return ReadState.Unread;
                return ReadState.Read;
            }
        }

        /// <inheritdoc/>
        public void Select()
        {
            if (IsConnected)
            {
                OpenChat();
            }
            else
            {
                JoinCall();
            }
        }

        /// <summary>
        /// Opens the voice channel as a text chat.
        /// </summary>
        public void OpenChat()
        {
            _messenger.Send(new SelectChannelMessage<IBindableSelectableChannel>(this));
        }

        /// <inheritdoc/>
        public void JoinCall() => _ = _discordService.JoinCall(Id, GuildId);

        /// <inheritdoc/>
        public void MarkRead() 
            => _ = _discordService.MarkRead(MessageChannel.Id, MessageChannel.LastMessageId ?? 0);

        /// <inheritdoc/>
        protected override void AckUpdate()
        {
            base.AckUpdate();
            OnPropertyChanged(nameof(MessageChannel));
        }
    }
}
