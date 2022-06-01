// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Messages;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
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

        /// <summary>
        /// Invoked when a channel is created.
        /// </summary>
        public event EventHandler<Channel>? ChannelCreated;

        /// <summary>
        /// Invoked when a channel is updated.
        /// </summary>
        public event EventHandler<Channel>? ChannelUpdated;

        /// <summary>
        /// Invoked when a channel is deleted.
        /// </summary>
        public event EventHandler<Channel>? ChannelDeleted;

        /// <summary>
        /// Invoked when a user logs out.
        /// </summary>
        public event Action? LoggedOut;

        /// <summary>
        /// Invoked when the gateway reconnects.
        /// </summary>
        public event Action? Reconnecting;

        /// <summary>
        /// Invoked when the gateway resumes.
        /// </summary>
        public event Action? Resuming;

        private void OnReady(Ready ready)
        {
            Guard.IsNotNull(ready, nameof(ready));

            AddSelfUser(ready.User);

            foreach (var guild in ready.Guilds)
            {
                // All child members are handled here
                AddGuild(guild);
            }

            foreach (var channel in ready.PrivateChannels)
            {
                AddChannel(channel);
            }

            foreach (var readState in ready.ReadStates)
            {
                AddReadState(readState);
            }

            foreach (var presence in ready.Presences)
            {
                AddPresence(presence);
            }

            foreach (var relationship in ready.Relationships)
            {
                AddRelationship(relationship);
            }

            UpdateSettings(ready.Settings);

            Guard.IsNotNull(_selfUser, nameof(_selfUser));

            LoggedIn?.Invoke(this, _selfUser);
        }

        private void OnMessageCreated(JsonMessage message)
        {
            if (_channelMap.TryGetValue(message.ChannelId, out Channel channel))
            {
                if (channel is IMessageChannel messageChannel)
                {
                    messageChannel.LastMessageId = message.Id;
                }
            }

            // TODO: Channel registration
            MessageCreated?.Invoke(this, new Message(message, this));
        }

        private void OnMessageUpdated(JsonMessage message)
        {
            MessageUpdated?.Invoke(this, new Message(message, this));
        }

        private void OnMessageAck(JsonMessageAck messageAck)
        {
            if (_channelMap.TryGetValue(messageAck.ChannelId, out Channel channel))
            {
                if (channel is IMessageChannel messageChannel)
                {
                    messageChannel.LastReadMessageId = messageAck.MessageId;
                }
            }

            // TODO: Channel registration
            MessageAck?.Invoke(this, new MessageAck(messageAck, this));
        }

        private void OnChannelCreated(JsonChannel jsonChannel)
        {
            var channel = AddChannel(jsonChannel);

            if (channel is not null)
            {
                ChannelCreated?.Invoke(this, channel);
            }
        }

        private void OnChannelUpdated(JsonChannel jsonChannel)
        {
            if (_channelMap.TryGetValue(jsonChannel.Id, out Channel channel))
            {
                channel.UpdateFromJsonChannel(jsonChannel);

                ChannelUpdated?.Invoke(this, channel);
            }
        }

        private void OnChannelDeleted(JsonChannel jsonChannel)
        {
            Channel? channel = RemoveChannel(jsonChannel.Id);

            if (channel is not null)
            {
                ChannelDeleted?.Invoke(this, channel);
            }
        }
    }
}
