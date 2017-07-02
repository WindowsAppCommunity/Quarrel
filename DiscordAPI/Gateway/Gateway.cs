using Discord_UWP.Authentication;
using Discord_UWP.Gateway.DownstreamEvents;
using Discord_UWP.Gateway.Sockets;
using Discord_UWP.Gateway.UpstreamEvents;
using Discord_UWP.SharedModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Discord_UWP.Gateway
{
    public class GatewayEventArgs<T> : EventArgs
    {
        public T EventData { get; }

        public GatewayEventArgs(T eventData)
        {
            EventData = eventData;
        }
    }

    public class Gateway : IGateway
    {
        private delegate void GatewayEventHandler(GatewayEvent gatewayEvent);

        private IDictionary<int, GatewayEventHandler> operationHandlers;
        private IDictionary<string, GatewayEventHandler> eventHandlers;

        private Ready? lastReady;
        private GatewayEvent? lastGatewayEvent;

        private readonly IWebMessageSocket _webMessageSocket;
        private readonly IAuthenticator _authenticator;
        private readonly GatewayConfig _gatewayConfig;

        public event EventHandler<GatewayEventArgs<Ready>> Ready;
        public event EventHandler<GatewayEventArgs<Resumed>> Resumed;

        public event EventHandler<GatewayEventArgs<Guild>> GuildCreated;
        public event EventHandler<GatewayEventArgs<Guild>> GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDelete>> GuildDeleted;

        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelCreated;
        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelUpdated;
        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelDeleted;

        public event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelCreated;
        public event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelDeleted;

        public event EventHandler<GatewayEventArgs<Message>> MessageCreated;
        public event EventHandler<GatewayEventArgs<Message>> MessageUpdated;
        public event EventHandler<GatewayEventArgs<MessageDelete>> MessageDeleted;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdate>> MessageReactionAdded;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdate>> MessageReactionRemoved;
        public event EventHandler<GatewayEventArgs<MessageReactionRemoveAll>> MessageReactionRemovedAll;

        public event EventHandler<GatewayEventArgs<GuildMemberAdd>> GuildMemberAdded;
        public event EventHandler<GatewayEventArgs<GuildMemberRemove>> GuildMemberRemoved;
        public event EventHandler<GatewayEventArgs<GuildMemberUpdate>> GuildMemberUpdated;
        public event EventHandler<GatewayEventArgs<GuildMemberChunk>> GuildMemberChunk;

        public event EventHandler<GatewayEventArgs<Friend>> RelationShipAdded;

        public event EventHandler<GatewayEventArgs<Presence>> PresenceUpdated;
        public event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;
        public event EventHandler<GatewayEventArgs<UserNote>> UserNoteUpdated;
        public Gateway(GatewayConfig config, IAuthenticator authenticator)
        {
            _webMessageSocket = new WebMessageSocket();
            _authenticator = authenticator;
            _gatewayConfig = config;

            eventHandlers = GetEventHandlers();
            operationHandlers = GetOperationHandlers();
          
            PrepareSocket();
        }

        private IDictionary<int, GatewayEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, GatewayEventHandler>
            {
                { OperationCode.Hello.ToInt(), OnHelloReceived },
                { OperationCode.Resume.ToInt(), OnResumeReceived }
            };
        }

        private IDictionary<string, GatewayEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, GatewayEventHandler>
            {
                { EventNames.READY, OnReady },
                { EventNames.GUILD_CREATED, OnGuildCreated },
                { EventNames.GUILD_UPDATED, OnGuildUpdated },
                { EventNames.GUILD_DELETED, OnGuildDeleted },
                { EventNames.MESSAGE_CREATED, OnMessageCreated },
                { EventNames.MESSAGE_UPDATED, OnMessageUpdated },
                { EventNames.MESSAGE_DELETED, OnMessageDeleted },
                { EventNames.MESSAGE_REACTION_ADD, OnMessageReactionAdd },
                { EventNames.MESSAGE_REACTION_REMOVE, OnMessageReactionRemove },
                { EventNames.MESSAGE_REACTION_REMOVE_ALL, OnMessageReactionRemoveAll },
                { EventNames.CHANNEL_CREATED, OnChannelCreated },
                { EventNames.CHANNEL_UPDATED, OnChannelUpdated },
                { EventNames.CHANNEL_DELETED, OnChannelDeleted },
                { EventNames.GUILD_MEMBER_ADDED, OnGuildMemberAdded},
                { EventNames.GUILD_MEMBER_REMOVED, OnGuildMemberRemoved },
                { EventNames.GUILD_MEMBER_UPDATED, OnGuildMemberUpdated },
                { EventNames.GUILD_MEMBER_CHUNK, OnGuildMemberChunk },
                { EventNames.PRESENCE_UPDATED, OnPresenceUpdated },
                { EventNames.TYPING_START, OnTypingStarted},
                { EventNames.RELATIONSHIP_ADD, OnRelationShipAdded },
                { EventNames.USER_NOTE_UPDATED, OnUserNoteUpdated }
            };
        }

        private void PrepareSocket()
        {
            _webMessageSocket.MessageReceived += OnSocketMessageReceived;
        }

        public async Task ConnectAsync()
        {
            await _webMessageSocket.ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "6"));
        }

        // TODO: good chance the socket will be disposed when attempting to resume so yah
        public async Task ResumeAsync()
        {
            var token = await _authenticator.GetToken();

            var resume = new GatewayResume
            {
                Token = token,
                SessionId = lastReady?.SessionId,
                LastSequenceNumberReceived = lastGatewayEvent?.SequenceNumber.Value ?? 0
            };

            await _webMessageSocket.SendJsonObjectAsync(resume);
        }

        public async void UpdateStatus(string onlinestatus, int? idleSince, Game? game)
        {
            status = new StatusUpdate()
            {
                Status = onlinestatus,
                IdleSince = idleSince,
                IsAFK = false,
                Game = game
            };
            await UpdateStatus();
        }
        
        public async Task RequestAllGuildMembers(string guildid)
        {
            var payload = new GuildMembersRequest()
            {
                GuildId = guildid,
                Query = "",
                Limit = 0
            };

            var Request = new GatewayEvent()
            {
                Operation = 8,
                Data = payload
            };
            await _webMessageSocket.SendJsonObjectAsync(Request);
        }

        public async void SubscribeToGuild(string[] guildIDs)
        {
            var identifyEvent = new GatewayEvent
            {
                Operation = OperationCode.SubscribeToGuild.ToInt(),
                Data = guildIDs
            };
            await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        private void OnSocketMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var gatewayEvent = JsonConvert.DeserializeObject<GatewayEvent>(args.Message);
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
        
        private void OnHelloReceived(GatewayEvent gatewayEvent)
        {
            IdentifySelfToGateway();
            BeginHeartbeatAsync(gatewayEvent.GetData<Hello>().HeartbeatInterval);
        }

        private async void IdentifySelfToGateway()
        {
            var identifyEvent = new GatewayEvent
            {
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify.ToInt(),
                Data = await GetIdentityAsync()
            };

            await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        private async Task<Identify> GetIdentityAsync()
        {
            return new Identify
            {
                Token = await _authenticator.GetToken(),
                Properties = GetClientProperties(),
                LargeThreshold = 250
            };
        }

        // TODO: move propeties to config
        private Properties GetClientProperties()
        {
            return new Properties
            {
                OS = "DISCORD-UWP",
                Device = "DISCORD-UWP",
                Browser = "DISCORD-UWP",
                Referrer = "",
                ReferringDomain = ""
            };
        }

        private void OnResumeReceived(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, Resumed);
        }

        private void OnReady(GatewayEvent gatewayEvent)
        {
            var ready = gatewayEvent.GetData<Ready>();
            lastReady = ready;

            FireEventOnDelegate(gatewayEvent, Ready);
        }

        private void OnMessageCreated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageCreated);
        }

        private void OnMessageUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageUpdated);
        }

        private void OnMessageDeleted(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageDeleted);
        }

        private void OnMessageReactionAdd(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionAdded);
        }

        private void OnMessageReactionRemove(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionRemoved);
        }

        private void OnMessageReactionRemoveAll(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionRemovedAll);
        }

        private void OnChannelCreated(GatewayEvent gatewayEvent)
        {
            if (IsChannelAGuildChannel(gatewayEvent))
            {
                FireEventOnDelegate(gatewayEvent, GuildChannelCreated);
            }
            else
            {
                FireEventOnDelegate(gatewayEvent, DirectMessageChannelCreated);
            }
        }

        private void OnChannelUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildChannelUpdated);
        }

        private void OnChannelDeleted(GatewayEvent gatewayEvent)
        {
            if (IsChannelAGuildChannel(gatewayEvent))
            {
                FireEventOnDelegate(gatewayEvent, GuildChannelDeleted);
            }
            else
            {
                FireEventOnDelegate(gatewayEvent, DirectMessageChannelDeleted);
            }
        }

        private bool IsChannelAGuildChannel(GatewayEvent gatewayEvent)
        {
            var dataAsJObject = gatewayEvent.Data as JObject;
            return dataAsJObject["guild_id"] != null;
        }

        private void OnGuildCreated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildCreated);
        }

        private void OnGuildUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildUpdated);
        }

        private void OnGuildDeleted(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildDeleted);
        }

        private void OnGuildMemberAdded(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberAdded);
        }

        private void OnGuildMemberRemoved(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberRemoved);
        }

        private void OnGuildMemberUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberUpdated);
        }

        private void OnGuildMemberChunk(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberChunk);
        }

        private void OnPresenceUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, PresenceUpdated);
        }

        private void OnTypingStarted(GatewayEvent gatewayEvent)
        {
            Debug.WriteLine("TYPING");
            FireEventOnDelegate(gatewayEvent, TypingStarted);
        }

        private void OnRelationShipAdded(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, RelationShipAdded);
        }

        private void OnUserNoteUpdated(GatewayEvent gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, UserNoteUpdated);
        }


        private void FireEventOnDelegate<TEventData>(GatewayEvent gatewayEvent, EventHandler<GatewayEventArgs<TEventData>> eventHandler)
        {
            var eventArgs = new GatewayEventArgs<TEventData>(gatewayEvent.GetData<TEventData>());
            eventHandler?.Invoke(this, eventArgs);
        }

        // TODO: dont while true and query connection state or use cancelation token or something
        private async void BeginHeartbeatAsync(int interval)
        {
            while (true)
            {
                await Task.Delay(interval);
                bool worked = false;
                int tried = 3;
                while (!worked && tried > 0)
                {
                    try
                    {
                        await SendHeartbeatAsync();
                        //await UpdateStatus();
                        worked = true;
                    }
                    catch
                    {
                        tried--;
                    }
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var heartbeatEvent = new GatewayEvent
                {
                    Operation = OperationCode.Heartbeat.ToInt(),
                    Data = lastGatewayEvent?.SequenceNumber ?? 0
                };

                await _webMessageSocket.SendJsonObjectAsync(heartbeatEvent);
            }
            catch
            {

            }
        }

        private async Task UpdateStatus()
        {
            var statusevent = new GatewayEvent()
            {
                Operation = 3,
                Data = status
            };
            await _webMessageSocket.SendJsonObjectAsync(statusevent);
        }

        StatusUpdate status = new StatusUpdate();
    }
}
