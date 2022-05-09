// Quarrel © 2022

using Discord.API.Models.Enums.Messages;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Messages.Embeds;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Messages.Embeds;
using Quarrel.Messages.Discord.Messages;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Generic;

namespace Quarrel.Bindables.Messages
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Messages.Message"/> that can be bound to the UI.
    /// </summary>
    public class BindableMessage : SelectableItem
    {
        private Message _message;
        private bool _isDeleted;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Message message,
            Message? previousMessage = null) :
            base(messenger, discordService, dispatcherService)
        {
            _message = message;
            _previousMessage = previousMessage;

            Users = new Dictionary<ulong, BindableUser?>();
            if (message.Author is not null)
            {
                Author = _discordService.GetUser(message.Author.Id);
                Users.Add(message.Author.Id, Author);

                if (message.GuildId.HasValue)
                {
                    AuthorMember = _discordService.GetGuildMember(message.Author.Id, message.GuildId.Value);
                }
            }

            foreach (var user in _message.Mentions)
            {
                if (!Users.ContainsKey(user.Id))
                {
                    Users.Add(user.Id, _discordService.GetUser(user.Id));
                }
            }

            Attachments = new BindableAttachment[_message.Attachments.Length];
            for (int i = 0; i < Attachments.Length; i++)
            {
                Attachments[i] = new BindableAttachment(messenger, discordService, dispatcherService, _message.Attachments[i]);
            }

            _messenger.Register<MessageUpdatedMessage>(this, (_, e) =>
            {
                if (Id == e.Message.Id)
                {
                    Message = e.Message;
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

        /// <inheritdoc/>
        public ulong Id => Message.Id;

        public Message Message
        {
            get => _message;
            set
            {
                SetProperty(ref _message, value);
                _dispatcherService.RunOnUIThread(AckUpdate);
            }
        }

        public string Content => Message.Content;

        public bool IsDeleted
        {
            get => _isDeleted;
            set => SetProperty(ref _isDeleted, value);
        }

        /// <summary>
        /// Gets the author of the message as a bindable user.
        /// </summary>
        public BindableUser? Author { get; }

        public BindableGuildMember? AuthorMember { get; }

        public Dictionary<ulong, BindableUser?> Users { get; }

        public BindableAttachment[] Attachments { get; }

        private Message? _previousMessage;

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

        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(Content));
        }
    }
}
