// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Messages;
using Quarrel.Messages.Discord;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using System.Collections.Generic;

namespace Quarrel.Bindables.Messages
{
    /// <summary>
    /// A wrapper of a <see cref="Client.Models.Messages.Message"/> that can be bound to the UI.
    /// </summary>
    public partial class BindableMessage : SelectableItem
    {
        private Message _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(
            IMessenger messenger,
            IDiscordService discordService,
            IDispatcherService dispatcherService,
            Message message) :
            base(messenger, discordService, dispatcherService)
        {
            _message = message;
            Author = _discordService.GetUser(message.Author.Id);

            Users = new Dictionary<ulong, BindableUser?>();
            Users.Add(message.Author.Id, Author);
            foreach (var user in _message.Mentions)
            {
                Users.Add(user.Id, _discordService.GetUser(user.Id));
            }

            if (message.GuildId.HasValue)
            {
                AuthorMember = _discordService.GetGuildMember(message.Author.Id, message.GuildId.Value);
            }

            _messenger.Register<MessageUpdatedMessage>(this, (_, e) =>
            {
                if (Id == e.Message.Id)
                {
                    Message = e.Message;
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
                AckUpdateRoot();
            }
        }

        public string Content => Message.Content;

        /// <summary>
        /// Gets the author of the message as a bindable user.
        /// </summary>
        public BindableUser? Author { get; }

        public BindableGuildMember? AuthorMember { get; }

        public Dictionary<ulong, BindableUser?> Users { get; }

        protected virtual void AckUpdate()
        {
            OnPropertyChanged(nameof(Message));
            OnPropertyChanged(nameof(Content));
        }

        private void AckUpdateRoot()
        {
            _dispatcherService.RunOnUIThread(() =>
            {
                AckUpdate();
            });
        }
    }
}
