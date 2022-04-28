// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Quarrel.Bindables.Abstract;
using Quarrel.Bindables.Users;
using Quarrel.Client.Models.Messages;
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
        [ObservableProperty]
        private Message _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableMessage"/> class.
        /// </summary>
        internal BindableMessage(IDiscordService discordService, IDispatcherService dispatcherService, Message message) :
            base(discordService, dispatcherService)
        {
            _message = message;
            Author = _discordService.GetUser(message.Author.Id);

            Users = new Dictionary<ulong, BindableUser?>();
            Users.Add(message.Author.Id, Author);
            foreach (var user in _message.Mentions)
            {
                Users.Add(user.Id, _discordService.GetUser(user.Id));
            }
        }

        /// <summary>
        /// Gets the author of the message as a bindable user.
        /// </summary>
        public BindableUser? Author { get; }

        public Dictionary<ulong, BindableUser?> Users { get; }
    }
}
