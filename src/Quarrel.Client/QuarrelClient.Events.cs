// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Exceptions;
using Discord.API.Gateways.Models.Channels;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Gateways.Models.Messages;
using Discord.API.Models.Json.Channels;
using Discord.API.Models.Json.Messages;
using Discord.API.Models.Json.Voice;
using Quarrel.Client.Models.Channels.Abstract;
using Quarrel.Client.Models.Channels.Interfaces;
using Quarrel.Client.Models.Messages;
using Quarrel.Client.Models.Users;
using Quarrel.Client.Models.Voice;
using System;

namespace Quarrel.Client
{
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
        /// Invoked when the voice connection handles an exception.
        /// </summary>
        public event EventHandler<SocketFrameException>? VoiceExceptionHandled;

        /// <summary>
        /// Invoked when the voice connection encounters a unknown operation
        /// </summary>
        public event EventHandler<int>? UnknownVoiceOperationEncountered;

        /// <summary>
        /// Invoked when the voice connection encounters a known operation but does not handle it
        /// </summary>
        public event EventHandler<int>? UnhandledVoiceOperationEncountered;

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
        /// Invoked when the voice server is updated.
        /// </summary>
        public event EventHandler<VoiceServerConfig>? VoiceServerUpdated;

        /// <summary>
        /// Invoked when a voice state is updated.
        /// </summary>
        public event EventHandler<VoiceState>? VoiceStateUpdated;

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

        private void OnReady(Ready arg)
        {
            Guard.IsNotNull(arg, nameof(arg));

            Self.SetSelfUser(arg.User);

            foreach (var guild in arg.Guilds)
            {
                // All child members are handled here
                Guilds.AddGuild(guild);
            }

            foreach (var channel in arg.PrivateChannels)
            {
                Channels.AddChannel(channel);
            }

            foreach (var readState in arg.ReadStates)
            {
                Channels.AddReadState(readState);
            }

            foreach (var presence in arg.Presences)
            {
                Users.AddPresence(presence);
            }

            foreach (var relationship in arg.Relationships)
            {
                Users.AddRelationship(relationship);
            }

            Self.UpdateSettings(arg.Settings);

            Guard.IsNotNull(Self.CurrentUser, nameof(Self.CurrentUser));

            LoggedIn?.Invoke(this, Self.CurrentUser);
        }

        private void OnMessageCreated(JsonMessage arg)
        {
            var channel = Channels.GetChannel(arg.ChannelId);
            if (channel is IMessageChannel messageChannel)
            {
                messageChannel.LastMessageId = arg.Id;
            }

            // TODO: Channel registration
            MessageCreated?.Invoke(this, new Message(arg, this));
        }

        private void OnMessageUpdated(JsonMessage arg)
            => MessageUpdated?.Invoke(this, new Message(arg, this));

        private void OnMessageDeleted(JsonMessageDeleted arg)
            => MessageDeleted?.Invoke(this, new MessageDeleted(arg, this));

        private void OnMessageAck(JsonMessageAck arg)
        {
            var channel = Channels.GetChannel(arg.ChannelId);
            if (channel is IMessageChannel messageChannel)
            {
                messageChannel.LastReadMessageId = arg.MessageId;
            }

            // TODO: Channel registration
            MessageAck?.Invoke(this, new MessageAck(arg, this));
        }

        private void OnChannelCreated(JsonChannel arg)
        {
            var channel = Channels.AddChannel(arg);
            if (channel is null) return;

            ChannelCreated?.Invoke(this, channel);
        }

        private void OnChannelUpdated(JsonChannel arg)
        {
            Channels.UpdateChannel(arg, out Channel channel);
            ChannelUpdated?.Invoke(this, channel);
        }

        private void OnChannelDeleted(JsonChannel arg)
        {
            Channel? channel = Channels.RemoveChannel(arg.Id);

            if (channel is null) return;

            ChannelDeleted?.Invoke(this, channel);
        }

        private void OnVoiceServerUpdated(JsonVoiceServerUpdate arg)
        {
            var config = new VoiceServerConfig(arg);
            Voice.UpdateVoiceServerConfig(config);

            VoiceServerUpdated?.Invoke(this, config);
        }

        private void OnVoiceStateUpdated(JsonVoiceState arg)
        {
            var state = new VoiceState(arg, this);

            if (arg.UserId == Self.CurrentUser?.Id)
            {
                Voice.UpdateVoiceState(arg);
            }

            VoiceStateUpdated?.Invoke(this, state);
        }
    }
}
