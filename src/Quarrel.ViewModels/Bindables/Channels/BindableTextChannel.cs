// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Channels.Abstract;
using Quarrel.Bindables.Channels.Enums;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Client;
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
    /// A wrapper of a <see cref="GuildTextChannel"/> that can be bound to the UI.
    /// </summary>
    public class BindableTextChannel : BindableGuildChannel, IBindableMessageChannel
    {
        internal BindableTextChannel(
            IMessenger messenger,
            IClipboardService clipboardService,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            GuildTextChannel channel,
            GuildMember selfMember,
            BindableCategoryChannel? parent = null) :
            base(messenger, clipboardService, discordService, quarrelClient, dispatcherService, channel, selfMember, parent)
        {
            SelectionCommand = new RelayCommand(Select);
            MarkAsReadCommand = new RelayCommand(MarkRead);
        }

        /// <inheritdoc/>
        public override bool IsAccessible => Permissions.ReadMessages;

        /// <inheritdoc/>
        public IMessageChannel MessageChannel => (IMessageChannel)Channel;

        /// <inheritdoc/>
        public GuildTextChannel TextChannel => (GuildTextChannel)Channel;

        /// <inheritdoc/>
        public RelayCommand SelectionCommand { get; }

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
        public void Select() => _messenger.Send(new SelectChannelMessage<IBindableSelectableChannel>(this));

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
