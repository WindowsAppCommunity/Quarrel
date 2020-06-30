using DiscordAPI.Authentication;
using DiscordAPI.Gateway.DownstreamEvents;
using DiscordAPI.Gateway.UpstreamEvents;
using DiscordAPI.Models;
using DiscordAPI.Models.Channels;
using DiscordAPI.Models.Guilds;
using DiscordAPI.Models.Messages;
using DiscordAPI.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAPI.Gateway
{
    public class GatewayEventArgs<T> : EventArgs
    {
        public T EventData { get; }

        public GatewayEventArgs(T eventData)
        {
            EventData = eventData;
        }
    }

    public class Gateway : IGatewayService
    {
        private delegate void GatewayEventHandler(SocketFrame gatewayEvent);

        private IReadOnlyDictionary<int, GatewayEventHandler> operationHandlers;
        private IReadOnlyDictionary<string, GatewayEventHandler> eventHandlers;

        private Ready lastReady;
        private int lastGatewayEventSeq;

        //private readonly IWebMessageSocket _webMessageSocket;
        private readonly IAuthenticator _authenticator;
        private readonly GatewayConfig _gatewayConfig;

        public event EventHandler<GatewayEventArgs<InvalidSession>> InvalidSession;

        public event EventHandler<GatewayEventArgs<Ready>> Ready;
        public event EventHandler<GatewayEventArgs<Resumed>> Resumed;
        public event EventHandler<Exception> GatewayClosed;

        public event EventHandler<GatewayEventArgs<Guild>> GuildCreated;
        public event EventHandler<GatewayEventArgs<Guild>> GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDelete>> GuildDeleted;
        public event EventHandler<GatewayEventArgs<GuildSync>> GuildSynced;

        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanAdded;
        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanRemoved;

        public event EventHandler<GatewayEventArgs<Channel>> ChannelCreated;
        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelUpdated;
        public event EventHandler<GatewayEventArgs<Channel>> ChannelDeleted;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientAdded;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientRemoved;

        public event EventHandler<GatewayEventArgs<Message>> MessageCreated;
        public event EventHandler<GatewayEventArgs<Message>> MessageUpdated;
        public event EventHandler<GatewayEventArgs<MessageDelete>> MessageDeleted;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdate>> MessageReactionAdded;
        public event EventHandler<GatewayEventArgs<MessageReactionUpdate>> MessageReactionRemoved;
        public event EventHandler<GatewayEventArgs<MessageReactionRemoveAll>> MessageReactionRemovedAll;
        public event EventHandler<GatewayEventArgs<MessageAck>> MessageAck;

        public event EventHandler<GatewayEventArgs<GuildMemberAdd>> GuildMemberAdded;
        public event EventHandler<GatewayEventArgs<GuildMemberRemove>> GuildMemberRemoved;
        public event EventHandler<GatewayEventArgs<GuildMemberUpdate>> GuildMemberUpdated;
        public event EventHandler<GatewayEventArgs<GuildMemberListUpdated>> GuildMemberListUpdated;
        public event EventHandler<GatewayEventArgs<GuildMembersChunk>> GuildMembersChunk;

        public event EventHandler<GatewayEventArgs<Friend>> RelationShipAdded;
        public event EventHandler<GatewayEventArgs<Friend>> RelationShipRemoved;
        public event EventHandler<GatewayEventArgs<Friend>> RelationShipUpdated;

        public event EventHandler<GatewayEventArgs<Presence>> PresenceUpdated;
        public event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;

        public event EventHandler<GatewayEventArgs<UserNote>> UserNoteUpdated;
        public event EventHandler<GatewayEventArgs<UserSettings>> UserSettingsUpdated;
        public event EventHandler<GatewayEventArgs<GuildSetting>> UserGuildSettingsUpdated;

        public event EventHandler<GatewayEventArgs<VoiceState>> VoiceStateUpdated;
        public event EventHandler<GatewayEventArgs<VoiceServerUpdate>> VoiceServerUpdated;

        public event EventHandler<GatewayEventArgs<SessionReplace[]>> SessionReplaced;

        WebSocketClient _socket;
        private MemoryStream _compressed;
        private DeflateStream _decompressor;
        public bool ConnectedSocket = false;

        private ILogger Logger { get; }
        private IServiceProvider ServiceProvider { get; }

        private int invalidSessionRetryCount;
        private string connectionUrl;

        public Gateway(IServiceProvider serviceProvider, GatewayConfig config, IAuthenticator authenticator)
        {
            ServiceProvider = serviceProvider;
            Logger = serviceProvider.GetService<ILogger<Gateway>>();

            CreateSocket();
            _compressed = new MemoryStream();
            if (UseCompression)
                _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);

            _authenticator = authenticator;
            _gatewayConfig = config;
            eventHandlers = GetEventHandlers();
            operationHandlers = GetOperationHandlers();
        }


        private void CreateSocket()
        {
            _socket?.Dispose();
            _socket = new WebSocketClient();
            _socket.TextMessage += HandleTextMessage;
            _socket.BinaryMessage += BinaryMessage;

            _socket.Closed += HandleClosed;

        }
        private void HandleClosed(Exception exception)
        {
            Logger?.LogError(new EventId(), exception, "HandleClosed");

            ConnectedSocket = false;
            if (exception is WebSocketClosedException ex)
            {
                Debug.WriteLine("Gateway closed with code " + ex.CloseCode + " and reason \"" + ex.Reason + "\"");
            }
            GatewayClosed?.Invoke(null, exception);
        }

        private void HandleTextMessage(string message)
        {
            using (var reader = new StringReader(message))
                HandleMessage(reader);
        }
        private void BinaryMessage(byte[] bytes, int index1, int count)
        {
            if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                Logger?.LogTrace("Binary message received.");
            }

            using (var ms = new MemoryStream(bytes))
            {

                ms.Position = 0;
                byte[] data = new byte[count];
                ms.Read(data, 0, count);
                int index = 0;
                using (var decompressed = new MemoryStream())
                {
                    if (data[0] == 0x78)
                    {
                        _compressed.Write(data, index + 2, count - 2);
                        _compressed.SetLength(count - 2);
                    }
                    else
                    {
                        _compressed.Write(data, index, count);
                        _compressed.SetLength(count);
                    }

                    _compressed.Position = 0;
                    _decompressor.CopyTo(decompressed);
                    _compressed.Position = 0;
                    decompressed.Position = 0;

                    using (var reader = new StreamReader(decompressed))
                    {
                        HandleMessage(reader);
                    }

                }
            }
        }

        private void HandleMessage(TextReader reader)
        {
            if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                Logger?.LogTrace("Handle Message.");
            }

            using (JsonReader jsreader = new JsonTextReader(reader))
            {
                JsonSerializer serializer = new JsonSerializer();
                SocketFrame frame = serializer.Deserialize<SocketFrame>(jsreader);

                if (Logger?.IsEnabled(LogLevel.Trace) ?? false)
                {
                    Logger?.LogTrace($"Frame:" +
                        $"\n\tOperation: {frame.Operation}" +
                        $"\n\tSequenceNumber: {frame.SequenceNumber}" +
                        $"\n\tType: {frame.Type}" +
                        $"\n\tPayload: {frame.Payload}"
                        );
                }

                if (frame.SequenceNumber.HasValue) lastGatewayEventSeq = frame.SequenceNumber.Value;
                if (operationHandlers.ContainsKey(frame.Operation.GetValueOrDefault()))
                {
                    operationHandlers[frame.Operation.GetValueOrDefault()](frame);
                }
                else if (frame.Type != null && eventHandlers.ContainsKey(frame.Type))
                {
                    eventHandlers[frame.Type](frame);
                }
                else
                {
                    Logger?.LogDebug($"Unknown message:" +
                                     $"\n\tOperation: {frame.Operation}" +
                                     $"\n\tSequenceNumber: {frame.SequenceNumber}" +
                                     $"\n\tType: {frame.Type}" +
                                     $"\n\tPayload: {frame.Payload}"
                    );
                }
            }
        }
        public async Task<bool> ConnectAsync(string connectionUrl)
        {
            try
            {
                invalidSessionRetryCount = 3;
                this.connectionUrl = connectionUrl;
                await _socket.ConnectAsync(connectionUrl);

                if (Logger?.IsEnabled(LogLevel.Information) ?? false)
                {
                    Logger?.LogInformation("Connected.");
                }

                return ConnectedSocket = true;
            }
            catch
            {
                if (Logger?.IsEnabled(LogLevel.Information) ?? false)
                {
                    Logger?.LogInformation("Connection Failed.");
                }

                return ConnectedSocket = false;
            }
        }

        public async Task SendMessageAsync(string message)
        {
#if DEBUG
            Debug.WriteLine(">>> " + (message.Length > 80 ? message.Substring(0, 80) + "..." : message));
#endif
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(bytes, 0, bytes.Length, true);
            }
            catch (Exception exception)
            {
                ConnectedSocket = false;
                if (exception is WebSocketClosedException ex)
                {
                    Debug.WriteLine("Gateway closed with code " + ex.CloseCode + " and reason \"" + ex.Reason + "\"");
                    GatewayClosed?.Invoke(null, ex);
                }
            }
        }

        public async Task Search(string query, List<string> guilds, int limit)
        {
            var frame = new SocketFrame()
            {
                Operation = 8,
                Payload = new Search
                {
                    query = query,
                    limit = limit,
                    guild_id = guilds
                }
            };
            await SendMessageAsync(JsonConvert.SerializeObject(frame));
            // var serialzedObject = ;
            // await _webMessageSocket.SendJsonObjectAsync(frame);
        }
        private IReadOnlyDictionary<int, GatewayEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, GatewayEventHandler>
            {
                { OperationCode.Hello.ToInt(), OnHelloReceived },
                { OperationCode.InvalidSession.ToInt(), OnInvalidSession }
            };
        }

        private IReadOnlyDictionary<string, GatewayEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, GatewayEventHandler>
            {
                { EventNames.READY, OnReady },
                { EventNames.GUILD_CREATED, OnGuildCreated },
                { EventNames.GUILD_UPDATED, OnGuildUpdated },
                { EventNames.GUILD_DELETED, OnGuildDeleted },
                { EventNames.GUILD_SYNC, OnGuildSynced },
                { EventNames.MESSAGE_CREATED, OnMessageCreated },
                { EventNames.MESSAGE_UPDATED, OnMessageUpdated },
                { EventNames.MESSAGE_DELETED, OnMessageDeleted },
                { EventNames.GUILD_BAN_ADDED,  OnGuildBanAdded },
                { EventNames.GUILD_BAN_REMOVED, OnGuildBanRemoved },
                { EventNames.MESSAGE_REACTION_ADD, OnMessageReactionAdd },
                { EventNames.MESSAGE_REACTION_REMOVE, OnMessageReactionRemove },
                { EventNames.MESSAGE_REACTION_REMOVE_ALL, OnMessageReactionRemoveAll },
                { EventNames.MESSAGE_ACK, OnMessageAck },
                { EventNames.CHANNEL_CREATED, OnChannelCreated },
                { EventNames.CHANNEL_UPDATED, OnChannelUpdated },
                { EventNames.CHANNEL_DELETED, OnChannelDeleted },
                { EventNames.GUILD_MEMBER_ADDED, OnGuildMemberAdded},
                { EventNames.GUILD_MEMBER_REMOVED, OnGuildMemberRemoved },
                { EventNames.GUILD_MEMBER_UPDATED, OnGuildMemberUpdated },
                { EventNames.GUILD_MEMBERS_CHUNK, OnGuildMembersChunk },
                { EventNames.GUILD_MEMBER_LIST_UPDATE, OnGuildMemberListUpdated },
                { EventNames.PRESENCE_UPDATED, OnPresenceUpdated },
                { EventNames.TYPING_START, OnTypingStarted},
                { EventNames.FRIEND_ADDED, OnRelationShipAdded },
                { EventNames.FRIEND_REMOVED, OnRelationShipRemoved },
                { EventNames.FRIEND_UPDATE, OnRelationShipUpdated },
                { EventNames.USER_NOTE_UPDATED, OnUserNoteUpdated },
                { EventNames.USER_GUILD_SETTINGS_UPDATED, OnUserGuildSettingsUpdated },
                { EventNames.USER_SETTINGS_UPDATED, OnUserSettingsUpdated },
                { EventNames.VOICE_STATE_UPDATED,  OnVoiceStatusUpdated },
                { EventNames.VOICE_SERVER_UPDATED, OnVoiceServerUpdated },
                { EventNames.SESSIONS_REPLACE, OnSessionReplaced },
                { EventNames.CHANNEL_RECIPIENT_ADD, OnChannelRecipientAdded },
                { EventNames.CHANNEL_RECIPIENT_REMOVE, OnChannelRecipientRemoved },
                { EventNames.RESUMED, OnResumeReceived },
            };
        }



        /* private void PrepareSocket()
         {
             _webMessageSocket.MessageReceived += OnSocketMessageReceived;
         }*/
        public static bool UseCompression = true;

        public async Task<bool> ConnectAsync()
        {
            string append = "";
            if (UseCompression)
            {
                append = "&compress=zlib-stream";
            }

            return await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "6", append));
        }

        // TODO: good chance the socket will be disposed when attempting to resume so yah
        public async Task<bool> ResumeAsync()
        {
            //Re-generate the socket
            CreateSocket();

            //Reconnect the socket
            TryResume = true;

            return await ConnectAsync();
            //The actual Resume payload is sent when a connection has been established
        }

        public async void SendResumeRequest()
        {
            //Reconnect the socket
            var token = _authenticator.GetToken();
            var payload = new GatewayResume
            {
                Token = token,
                SessionId = lastReady?.SessionId,
                LastSequenceNumberReceived = lastGatewayEventSeq,
            };
            await SendMessageAsync("{\"op\":6,\"d\":" + JsonConvert.SerializeObject(payload) + "}");
            // await _webMessageSocket.SendJsonObjectAsync(resume);
        }

        public void UpdateStatus(GameBase game)
        {
            UpdateStatus(null, null, game);
        }

        public async void UpdateStatus(string onlinestatus, int? idleSince, GameBase game)
        {
            if (game != null)
            {
                if (game.Type == 3)
                {
                    game.Type = 0;
                }
                else if (game.IsCustom)
                {
                    game = new Game() { Name = "Custom Status", Type = 4, State = game.Name };
                }
                if (game is Game game1)
                {
                    game1.SessionId = lastReady.SessionId;
                }
            }
            await UpdateStatus(new StatusUpdate()
            {
                Status = onlinestatus,
                IdleSince = idleSince,
                IsAFK = false,
                Game = game
            });
        }

        public async Task RequestAllGuildMembers(string guildid)
        {
            var payload = new GuildMembersRequest()
            {
                GuildId = guildid,
                Query = "",
                Limit = 0
            };

            var request = new SocketFrame()
            {
                Operation = (int)OperationCode.RequestGuildMembers,
                Payload = payload
            };
            await SendMessageAsync(JsonConvert.SerializeObject(request));
            //await _webMessageSocket.SendJsonObjectAsync(request);
        }

        public async Task VoiceStatusUpdate(string guildId, string channelId, bool selfMute, bool selfDeaf)
        {
            var payload = new VoiceStatusUpdate()
            {
                GuildId = guildId,
                ChannelId = channelId,
                Deaf = selfDeaf,
                Mute = selfMute
            };

            var request = new SocketFrame()
            {
                Operation = (int)OperationCode.VoiceStateUpdate,
                Payload = payload
            };
            await SendMessageAsync(JsonConvert.SerializeObject(request));
            //await _webMessageSocket.SendJsonObjectAsync(request);
        }

        /// <summary>
        /// Subscribe to various channel-specific events (most notably "Typing")
        /// </summary>
        /// <param name="channelIds"></param>
        /// <returns>True if the payload was valid, false if it wasn't</returns>
        public async Task<bool> SubscribeToGuild(string[] channelIds)
        {
            if (channelIds.Length > 193) //This is really, really random, but it's apparently the maximum array size for the op12 event.
            {
                return false;
            }
            var identifyEvent = new SocketFrame
            {
                Operation = OperationCode.SubscribeToGuild.ToInt(),
                Payload = channelIds
            };
            await SendMessageAsync(JsonConvert.SerializeObject(identifyEvent));
            return true;
        }

        public async Task SubscribeToGuildLazy(string guildId, IReadOnlyDictionary<string, IEnumerable<int[]>> channels = null, IEnumerable<string> members = null)
        {
            var updateGuildSubscriptions = new SocketFrame
            {
                Operation = (int)OperationCode.UpdateGuildSubscriptions,
                Payload = new UpdateGuildSubscriptions()
                {
                    GuildId = guildId,
                    Channels = channels,
                    Members = members,
                    Typing = true
                }
            };
            await SendMessageAsync(JsonConvert.SerializeObject(updateGuildSubscriptions, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }
            ));
        }
        public async Task RequestGuildMembers(IEnumerable<string> guildIds, string query, int? limit, bool? presences, IEnumerable<string> userIds)
        {
            var updateGuildSubscriptions = new SocketFrame
            {
                Operation = (int)OperationCode.RequestGuildMembers,
                Payload = new GuildRequestMembers()
                {
                    GuildIds = guildIds,
                    Query = query,
                    Limit = limit,
                    Presences = presences,
                    UserIds = userIds
                }
            };
            await SendMessageAsync(JsonConvert.SerializeObject(updateGuildSubscriptions, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }
            ));
        }

        private bool TryResume = false;
        private void OnHelloReceived(SocketFrame gatewayEvent)
        {
            if (TryResume)
            {
                SendResumeRequest();
                TryResume = false;
            }
            else
            {
                IdentifySelfToGateway();
                BeginHeartbeatAsync(gatewayEvent.GetData<Hello>().HeartbeatInterval);
            }
        }

        private async void IdentifySelfToGateway()
        {
            var identifyEvent = new SocketFrame
            {
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify.ToInt(),
                Payload = GetIdentityAsync()
            };
            await SendMessageAsync(JsonConvert.SerializeObject(identifyEvent));
            // await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        private Identify GetIdentityAsync()
        {
            return new Identify
            {
                Token = _authenticator.GetToken(),
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


        #region OnEvents

        private void OnResumeReceived(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, Resumed);
        }

        private void OnReady(SocketFrame gatewayEvent)
        {
            var ready = gatewayEvent.GetData<Ready>();
            lastReady = ready;


            FireEventOnDelegate(gatewayEvent, Ready);
        }
        private void OnSessionReplaced(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, SessionReplaced);
        }

        private void OnMessageCreated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageCreated);
        }

        private void OnMessageUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageUpdated);
        }

        private void OnMessageDeleted(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageDeleted);
        }

        private void OnMessageReactionAdd(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionAdded);
        }

        private void OnMessageReactionRemove(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionRemoved);
        }

        private void OnMessageReactionRemoveAll(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageReactionRemovedAll);
        }

        private void OnMessageAck(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, MessageAck);
        }

        private void OnChannelCreated(SocketFrame gatewayEvent)
        {
            GatewayEventArgs<Channel> eventArgs;
            if (IsChannelAGuildChannel(gatewayEvent))
                eventArgs = new GatewayEventArgs<Channel>(gatewayEvent.GetData<GuildChannel>());
            else
                eventArgs = new GatewayEventArgs<Channel>(gatewayEvent.GetData<DirectMessageChannel>());

            ChannelCreated?.Invoke(this, eventArgs);
        }

        private void OnChannelUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildChannelUpdated);
        }

        private void OnChannelDeleted(SocketFrame gatewayEvent)
        {
            GatewayEventArgs<Channel> eventArgs;
            if (IsChannelAGuildChannel(gatewayEvent))
                eventArgs = new GatewayEventArgs<Channel>(gatewayEvent.GetData<GuildChannel>());
            else
                eventArgs = new GatewayEventArgs<Channel>(gatewayEvent.GetData<DirectMessageChannel>());

            ChannelDeleted?.Invoke(this, eventArgs);
        }

        private void OnGuildCreated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildCreated);
        }

        private void OnGuildUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildUpdated);
        }

        private void OnGuildDeleted(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildDeleted);
        }

        private void OnGuildSynced(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildSynced);
        }

        private void OnGuildBanAdded(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildBanAdded);
        }

        private void OnGuildBanRemoved(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildBanRemoved);
        }

        private void OnGuildMemberAdded(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberAdded);
        }

        private void OnGuildMemberRemoved(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberRemoved);
        }

        private void OnGuildMemberUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberUpdated);
        }

        private void OnGuildMemberListUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberListUpdated);
        }

        private void OnGuildMembersChunk(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMembersChunk);
        }

        private void OnChannelRecipientAdded(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, ChannelRecipientAdded);
        }
        private void OnChannelRecipientRemoved(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, ChannelRecipientRemoved);
        }


        private void OnPresenceUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, PresenceUpdated);
        }

        private void OnTypingStarted(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, TypingStarted);
        }

        private void OnRelationShipAdded(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, RelationShipAdded);
        }

        private void OnRelationShipRemoved(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, RelationShipRemoved);
        }

        private void OnRelationShipUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, RelationShipUpdated);
        }

        private void OnUserNoteUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, UserNoteUpdated);
        }

        private void OnVoiceStatusUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, VoiceStateUpdated);
        }

        private void OnVoiceServerUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, VoiceServerUpdated);
        }

        private void OnUserGuildSettingsUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, UserGuildSettingsUpdated);
        }

        private void OnUserSettingsUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, UserSettingsUpdated);
        }

        private void FireEventOnDelegate<TEventData>(SocketFrame gatewayEvent, EventHandler<GatewayEventArgs<TEventData>> eventHandler)
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
                var heartbeatEvent = new SocketFrame
                {
                    Operation = OperationCode.Heartbeat.ToInt(),
                    Payload = lastGatewayEventSeq
                };

                if (DateTime.Now.Day == 1 && DateTime.Now.Month == 4) //April 1st
                {
                    //App.PlayHeartBeat();
                }
                await SendMessageAsync(JsonConvert.SerializeObject(heartbeatEvent));
                //await _webMessageSocket.SendJsonObjectAsync(heartbeatEvent);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }

        private async Task UpdateStatus(StatusUpdate status)
        {
            var statusevent = new SocketFrame()
            {
                Operation = 3,
                Payload = status
            };
            await SendMessageAsync(JsonConvert.SerializeObject(statusevent));
            //await _webMessageSocket.SendJsonObjectAsync(statusevent);
        }

        private void OnInvalidSession(SocketFrame gatewayEvent)
        {
            if (invalidSessionRetryCount > 0)
            {
                invalidSessionRetryCount--;
                _ = ConnectAsync(connectionUrl);
            }
            else
            {
                ConnectedSocket = false;

                FireEventOnDelegate(gatewayEvent, InvalidSession);
            }
        }

        private bool IsChannelAGuildChannel(SocketFrame gatewayEvent)
        {
            var dataAsJObject = gatewayEvent.Payload as JObject;
            return dataAsJObject["guild_id"] != null;
        }

        #endregion
    }
}
