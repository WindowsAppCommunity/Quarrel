using Discord_UWP.Authentication;
using Discord_UWP.SharedModels;
using Discord_UWP.Sockets;
using Discord_UWP.Voice.DownstreamEvents;
using Discord_UWP.Voice.UpstreamEvents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_UWP.Voice
{
    public class VoiceConnectionEventArgs<T> : EventArgs
    {
        public T EventData { get; }

        public VoiceConnectionEventArgs(T eventData)
        {
            EventData = eventData;
        }
    }

    public class VoiceConnection
    {
        private delegate void VoiceConnectionEventHandler(SocketFrame gatewayEvent);

        private IDictionary<int, VoiceConnectionEventHandler> operationHandlers;
        private IDictionary<string, VoiceConnectionEventHandler> eventHandlers;

        private Ready? lastReady;
        private SocketFrame? lastGatewayEvent;

        private readonly IWebMessageSocket _webMessageSocket;
        private readonly VoiceState State;
        private readonly VoiceServerUpdate _voiceServerConfig;

        event EventHandler<VoiceConnectionEventArgs<Ready>> Ready;

        public VoiceConnection(VoiceServerUpdate config)
        {
            _webMessageSocket = new WebMessageSocket();
            _voiceServerConfig = config;

            eventHandlers = GetEventHandlers();
            operationHandlers = GetOperationHandlers();

            PrepareSocket();
        }

        private void PrepareSocket()
        {
            _webMessageSocket.MessageReceived += OnSocketMessageReceived;
        }

        public async Task ConnectAsync()
        {
            await _webMessageSocket.ConnectAsync(_voiceServerConfig.GetConnectionUrl());
        }

        private async void IdentifySelfToGateway()
        {
            var identifyEvent = new SocketFrame
            {
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify.ToInt(),
                Payload = GetIdentityAsync()
            };

            await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        private IDictionary<int, VoiceConnectionEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, VoiceConnectionEventHandler>
            {
                //{ OperationCode.Identify.ToInt(), OnHelloReceived },
                //{ OperationCode.Ready.ToInt(), OnResumeReceived }
            };
        }

        private IDictionary<string, VoiceConnectionEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, VoiceConnectionEventHandler>
            {
                { EventNames.READY, OnReady },
                //{ EventNames.GUILD_CREATED, OnGuildCreated },
                //{ EventNames.GUILD_UPDATED, OnGuildUpdated },
                //{ EventNames.GUILD_DELETED, OnGuildDeleted },
                //{ EventNames.MESSAGE_CREATED, OnMessageCreated },
                //{ EventNames.MESSAGE_UPDATED, OnMessageUpdated },
                //{ EventNames.MESSAGE_DELETED, OnMessageDeleted },
                //{ EventNames.GUILD_BAN_ADDED,  OnGuildBanAdded },
                //{ EventNames.GUILD_BAN_REMOVED, OnGuildBanRemoved },
                //{ EventNames.MESSAGE_REACTION_ADD, OnMessageReactionAdd },
                //{ EventNames.MESSAGE_REACTION_REMOVE, OnMessageReactionRemove },
                //{ EventNames.MESSAGE_REACTION_REMOVE_ALL, OnMessageReactionRemoveAll },
                //{ EventNames.MESSAGE_ACK, OnMessageAck },
                //{ EventNames.CHANNEL_CREATED, OnChannelCreated },
                //{ EventNames.CHANNEL_UPDATED, OnChannelUpdated },
                //{ EventNames.CHANNEL_DELETED, OnChannelDeleted },
                //{ EventNames.GUILD_MEMBER_ADDED, OnGuildMemberAdded},
                //{ EventNames.GUILD_MEMBER_REMOVED, OnGuildMemberRemoved },
                //{ EventNames.GUILD_MEMBER_UPDATED, OnGuildMemberUpdated },
                //{ EventNames.GUILD_MEMBER_CHUNK, OnGuildMemberChunk },
                //{ EventNames.PRESENCE_UPDATED, OnPresenceUpdated },
                //{ EventNames.TYPING_START, OnTypingStarted},
                //{ EventNames.FRIEND_ADDED, OnRelationShipAdded },
                //{ EventNames.FRIEND_REMOVED, OnRelationShipRemoved },
                //{ EventNames.FRIEND_UPDATE, OnRelationShipUpdated },
                //{ EventNames.USER_NOTE_UPDATED, OnUserNoteUpdated },
                //{ EventNames.USER_SETTINGS_UPDATED, OnUserSettingsUpdated },
                //{ EventNames.VOICE_STATE_UPDATED,  OnVoiceStatusUpdated }
            };
        }

        private Identify GetIdentityAsync()
        {
            return new Identify
            {
                ServerId = _voiceServerConfig.ServerId,
                SessionId = State.SessionId,
                Token = _voiceServerConfig.Token,
                UserId = State.UserId
            };
        }

        private void OnSocketMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var gatewayEvent = JsonConvert.DeserializeObject<SocketFrame>(args.Message);
            lastGatewayEvent = gatewayEvent;

            if (operationHandlers.ContainsKey(gatewayEvent.Operation.GetValueOrDefault()))
            {
                operationHandlers[gatewayEvent.Operation.GetValueOrDefault()](gatewayEvent);
            }

            if (gatewayEvent.Type != null && eventHandlers.ContainsKey(gatewayEvent.Type))
            {
                eventHandlers[gatewayEvent.Type](gatewayEvent);
            }
        }

        #region Event
        private void OnReady(SocketFrame gatewayEvent)
        {
            var ready = gatewayEvent.GetData<Ready>();
            lastReady = ready;

            FireEventOnDelegate(gatewayEvent, Ready);
        }
        #endregion

        private void FireEventOnDelegate<TEventData>(SocketFrame gatewayEvent, EventHandler<VoiceConnectionEventArgs<TEventData>> eventHandler)
        {
            var eventArgs = new VoiceConnectionEventArgs<TEventData>(gatewayEvent.GetData<TEventData>());
            eventHandler?.Invoke(this, eventArgs);
        }
    }
}
