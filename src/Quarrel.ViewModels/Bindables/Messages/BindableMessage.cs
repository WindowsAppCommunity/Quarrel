// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Channels.Interfaces;
using Quarrel.Bindables.Messages.Embeds;
using Quarrel.Bindables.Users;
using Quarrel.Client;
using Quarrel.Client.Models.Messages;
using Quarrel.Messages.Discord.Messages;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Generic;
using System.Linq;

namespace Quarrel.Bindables.Messages
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Messages.Message"/> that can be bound to the UI.
    /// </summary>
    public class BindableMessage : SelectableItem
    {
        private readonly IClipboardService _clipboardService;

        private Message _message;
        private Message? _previousMessage;
        private bool _isDeleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(
            IMessenger messenger,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService,
            IClipboardService clipboardService,
            IBindableMessageChannel channel,
            Message message,
            Message? previousMessage = null) :
            base(messenger, discordService, quarrelClient, dispatcherService)
        {
            _clipboardService = clipboardService;

            _message = message;
            _previousMessage = previousMessage;
            Channel = channel;

            Users = new Dictionary<ulong, BindableUser?>();
            if (message.Author is not null)
            {

                var user = _quarrelClient.Users.GetUser(message.Author.Id);
                Author = user != null ? new BindableUser(_messenger, _discordService, _quarrelClient, _dispatcherService, user) : null;
                Users.Add(message.Author.Id, Author);

                if (message.GuildId.HasValue)
                {
                    AuthorMember = GetGuildMember(message.Author.Id, message.GuildId.Value);
                }
            }

            foreach (var user in _message.Mentions)
            {
                if (!Users.ContainsKey(user.Id))
                {
                    var mentionedUser = _quarrelClient.Users.GetUser(user.Id);
                    Users.Add(user.Id, mentionedUser != null ? new BindableUser(_messenger, _discordService, _quarrelClient, _dispatcherService, mentionedUser) : null);
                }
            }

            Attachments = new BindableAttachment[_message.Attachments.Length];
            for (int i = 0; i < Attachments.Length; i++)
            {
                Attachments[i] = new BindableAttachment(messenger, discordService, quarrelClient, dispatcherService, _message.Attachments[i]);
            }

            Reactions = new BindableReaction[_message.Reactions.Length];
            for (int i = 0; i < Reactions.Length; i++)
            {
                Reactions[i] = new BindableReaction(messenger, discordService, quarrelClient, dispatcherService, _message.Reactions[i]);
            }

            MarkLastReadCommand = new RelayCommand(() => _discordService.MarkRead(ChannelId, Id));
            CopyIdCommand = new RelayCommand(() => _clipboardService.Copy($"{Id}"));
            CopyLinkCommand = new RelayCommand(() => _clipboardService.Copy($"{message.MessageUri}"));
            DeleteCommand = new RelayCommand(() => _discordService.DeleteMessage(ChannelId, Id));

            _messenger.Register<MessageUpdatedMessage>(this, (_, e) =>
            {
                if (Id == e.Message.Id)
                {
                    _dispatcherService.RunOnUIThread(() => Message = e.Message);
                }
            });

            _messenger.Register<MessageDeletedMessage>(this, (_, e) =>
            {
                if (Id == e.MessageId)
                {
                    _dispatcherService.RunOnUIThread(() => IsDeleted = true);
                }
            });
        }

        public BindableGuildMember? GetGuildMember(ulong userId, ulong guildId)
        {
            var member = _quarrelClient.Members.GetGuildMember(userId, guildId);
            if (member is not null)
            {
                return new BindableGuildMember(_messenger, _discordService, _quarrelClient, _dispatcherService, member);
            }

            return null;
        }

        /// <inheritdoc/>
        public ulong Id => Message.Id;

        /// <inheritdoc/>
        public ulong ChannelId => Message.ChannelId;

        /// <summary>
        /// Gets or sets the wrapped <see cref="Client.Models.Messages.Message"/>.
        /// </summary>
        public Message Message
        {
            get => _message;
            set
            {
                SetProperty(ref _message, value);
                AckUpdate();
            }
        }

        /// <summary>
        /// Gets or sets the previous message.
        /// </summary>
        public Message PreviousMessage
        {
            get => _message;
            set
            {
                SetProperty(ref _previousMessage, value);
                OnPropertyChanged(nameof(IsContinuation));
            }
        }

        /// <summary>
        /// Gets whether or not the message has been deleted.
        /// </summary>
        public bool IsDeleted
        {
            get => _isDeleted;
            private set => SetProperty(ref _isDeleted, value);
        }

        /// <summary>
        /// Gets the <see cref="IBindableMessageChannel"/> that the message belongs to.
        /// </summary>
        public IBindableMessageChannel Channel { get; }

        /// <summary>
        /// Gets the author of the message as a bindable user.
        /// </summary>
        public BindableUser? Author { get; }

        /// <summary>
        /// Gets the author of the message as a bindable guild memeber.
        /// </summary>
        public BindableGuildMember? AuthorMember { get; }

        /// <summary>
        /// Gets a dictionary of bindable users relavent to 
        /// </summary>
        public Dictionary<ulong, BindableUser?> Users { get; }

        /// <summary>
        /// Gets the message attachments.
        /// </summary>
        public BindableAttachment[] Attachments { get; }

        /// <summary>
        /// Gets the message reactions.
        /// </summary>
        public BindableReaction[] Reactions { get; }

        /// <summary>
        /// Gets whether or not the message is a continuation.
        /// </summary>
        public bool IsContinuation => !(
            _previousMessage == null ||
            _message.Type is MessageType.ApplicationCommand or MessageType.ContextMenuCommand || 
            (_message.Type != MessageType.Default && 
                (_previousMessage.Type is MessageType.Default or MessageType.Reply ||
                 _previousMessage is { Type: MessageType.ApplicationCommand or MessageType.ContextMenuCommand, Interaction: null })) ||
            (_message.Type == MessageType.Default && 
                ((_previousMessage.Type is not MessageType.Default or MessageType.Reply && 
                 _previousMessage is not { Type: MessageType.ApplicationCommand or MessageType.ContextMenuCommand, Interaction: null }) ||
                 _previousMessage.Author?.Id != _message.Author?.Id ||
                 _previousMessage.Flags?.HasFlag(MessageFlags.EPHEMERAL) != _message.Flags?.HasFlag(MessageFlags.EPHEMERAL) ||
                 _message.WebhookId != null && _previousMessage.Author?.Username != _message.Author?.Username ||
                 _message.Timestamp.ToUnixTimeMilliseconds() - _previousMessage.Timestamp.ToUnixTimeMilliseconds() >  7 * 60 * 1000)));

        /// <summary>
        /// Gets a value indicating whether or not the current user is mentioned in the message.
        /// </summary>
        public bool MentionsMe => Message.MentionsEveryone ||
                                  Message.Mentions.Any(x => x.Id == _discordService.MyId);

        /// <summary>
        /// Gets whether or not the user can delete the message.
        /// </summary>
        // TODO: Properly handle deletable condition
        public bool CanDelete => Message.IsOwn;

        /// <summary>
        /// Gets a command that marks the message as the last read message in the channel.
        /// </summary>
        public RelayCommand MarkLastReadCommand { get; }

        /// <summary>
        /// Gets a command that copies the message id to the clipboard.
        /// </summary>
        public RelayCommand CopyIdCommand { get; }

        /// <summary>
        /// Gets a command that copies the message link to the clipboard.
        /// </summary>
        public RelayCommand CopyLinkCommand { get; }

        /// <summary>
        /// Gets a command that deletes the message.
        /// </summary>
        public RelayCommand DeleteCommand { get; }

        /// <inheritdoc/>
        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Message));
        }
    }
}
