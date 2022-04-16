// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Users;
using System;

namespace Quarrel.Client
{
    /// <inheritdoc/>
    public partial class QuarrelClient
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
        /// Invoked when the gateway encounters a unknown operation
        /// </summary>
        public event EventHandler<int>? UnknownGatewayOperationEncountered;
        
        /// <summary>
        /// Invoked when the gateway encounters a unknown event
        /// </summary>
        public event EventHandler<string>? UnknownGatewayEventEncountered;

        /// <summary>
        /// Invoked when the gateway encounters a known event
        /// </summary>
        public event EventHandler<string>? KnownGatewayEventEncountered;

        /// <summary>
        /// Invoked when the gateway encounters a known operation but does not handle it
        /// </summary>
        public event EventHandler<int>? UnhandledGatewayOperationEncountered;

        /// <summary>
        /// Invoked when the gateway encounters a known event but does not handle it
        /// </summary>
        public event EventHandler<string>? UnhandledGatewayEventEncountered;

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
    }
}
