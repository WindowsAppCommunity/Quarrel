// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Messages;
using Discord.API.Models.Users;
using System;

namespace Discord.API
{
    public partial class DiscordClient
    {
        public event EventHandler<SelfUser> LoggedIn;

        public event EventHandler<Message>? MessageCreated;
        public event EventHandler<Message>? MessageUpdated;
        public event EventHandler<MessageDeleted>? MessageDeleted;
        public event EventHandler<MessageAck>? MessageAck;

        private void RegisterEvents()
        {
            Guard.IsNotNull(_gateway, nameof(_gateway));

            _gateway.Ready += OnReady;

            _gateway.MessageCreated += OnMessageCreated;
            _gateway.MessageUpdated += OnMessageUpdated;
            _gateway.MessageDeleted += (s, e) => ForwardEvent(e.EventData, MessageDeleted);
            _gateway.MessageAck += OnMessageAck;
        }

        private void ForwardEvent<T>(T? arg, EventHandler<T>? eventHandler)
            where T : class
        {
            Guard.IsNotNull(arg, nameof(arg));
            eventHandler?.Invoke(this, arg);
        }
    }
}
