// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Managed.Messages;
using Discord.API.Models.Messages;
using Discord.API.Models.Users;
using System;

namespace Discord.API
{
    /// <inheritdoc/>
    public partial class DiscordClient
    {
        /// <summary>
        /// Invoked when an http exception is handled.
        /// </summary>
        public event EventHandler<Exception>? HttpExceptionHandled;

        /// <summary>
        /// Invoked when the gateway handles an exception.
        /// </summary>
        public event EventHandler<SocketFrameException>? GatewayExceptionHandled;

        /// <summary>
        /// Invoked when the user logs in.
        /// </summary>
        public event EventHandler<SelfUser>? LoggedIn;

        /// <summary>
        /// Invoked when a message is created.
        /// </summary>
        public event EventHandler<Message>? MessageCreated;
        
        /// <summary>
        /// Invoked when a message is updated.
        /// </summary>
        public event EventHandler<Message>? MessageUpdated;

        /// <summary>
        /// Invoked when a message is deleted.
        /// </summary>
        public event EventHandler<MessageDeleted>? MessageDeleted;

        /// <summary>
        /// Invoked when a message is marked as read.
        /// </summary>
        public event EventHandler<MessageAck>? MessageAck;

        private void RegisterEvents()
        {
            Guard.IsNotNull(_gateway, nameof(_gateway));

            _gateway.Ready += OnReady;

            _gateway.MessageCreated += OnMessageCreated;
            _gateway.MessageUpdated += OnMessageUpdated;
            _gateway.MessageDeleted += (s, e) => ForwardEvent(e.EventData is not null ? new MessageDeleted(e.EventData, this) : null, MessageDeleted);
            _gateway.MessageAck += OnMessageAck;
            _gateway.UnhandledMessageEncountered += (s, e) => ForwardEvent(e, GatewayExceptionHandled);
        }

        private void ForwardEvent<T>(T? arg, EventHandler<T>? eventHandler)
            where T : class
        {
            Guard.IsNotNull(arg, nameof(arg));
            eventHandler?.Invoke(this, arg);
        }
    }
}
