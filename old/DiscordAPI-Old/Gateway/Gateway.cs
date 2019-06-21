using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using System.IO;
using System.IO.Compression;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.Web;
using DiscordAPI.Authentication;
using DiscordAPI.Sockets;
using DiscordAPI.API.Gateway.DownstreamEvents;
using DiscordAPI.API.Gateway.UpstreamEvents;
using DiscordAPI.SharedModels;

namespace DiscordAPI.API.Gateway
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

        private IDictionary<int, GatewayEventHandler> operationHandlers;
        private IDictionary<string, GatewayEventHandler> eventHandlers;

        private Ready lastReady;
        private int lastGatewayEventSeq;

        //private readonly IWebMessageSocket _webMessageSocket;
        private readonly IAuthenticator _authenticator;
        private readonly GatewayConfig _gatewayConfig;

        public event EventHandler<GatewayEventArgs<Ready>> Ready;
        public event EventHandler<GatewayEventArgs<Resumed>> Resumed;
        public event EventHandler<WebSocketClosedEventArgs> GatewayClosed;

        public event EventHandler<GatewayEventArgs<SharedModels.Guild>> GuildCreated;
        public event EventHandler<GatewayEventArgs<SharedModels.Guild>> GuildUpdated;
        public event EventHandler<GatewayEventArgs<GuildDelete>> GuildDeleted;
        public event EventHandler<GatewayEventArgs<GuildSync>> GuildSynced;

        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanAdded;
        public event EventHandler<GatewayEventArgs<GuildBanUpdate>> GuildBanRemoved;

        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelCreated;
        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelUpdated;
        public event EventHandler<GatewayEventArgs<GuildChannel>> GuildChannelDeleted;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientAdded;
        public event EventHandler<GatewayEventArgs<ChannelRecipientUpdate>> ChannelRecipientRemoved;

        public event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelCreated;
        public event EventHandler<GatewayEventArgs<DirectMessageChannel>> DirectMessageChannelDeleted;

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
        public event EventHandler<GatewayEventArgs<GuildMemberChunk>> GuildMemberChunk;

        public event EventHandler<GatewayEventArgs<Friend>> RelationShipAdded;
        public event EventHandler<GatewayEventArgs<Friend>> RelationShipRemoved;
        public event EventHandler<GatewayEventArgs<Friend>> RelationShipUpdated;

        public event EventHandler<GatewayEventArgs<Presence>> PresenceUpdated;
        public event EventHandler<GatewayEventArgs<TypingStart>> TypingStarted;
        public event EventHandler<GatewayEventArgs<UserNote>> UserNoteUpdated;
        public event EventHandler<GatewayEventArgs<UserSettings>> UserSettingsUpdated;

        public event EventHandler<GatewayEventArgs<VoiceState>> VoiceStateUpdated;
        public event EventHandler<GatewayEventArgs<VoiceServerUpdate>> VoiceServerUpdated;

        public event EventHandler<GatewayEventArgs<SessionReplace>> SessionReplaced;

        MessageWebSocket _socket;
        private MemoryStream _compressed;
        private DeflateStream _decompressor;
        public bool ConnectedSocket = false;
        public Gateway(GatewayConfig config, IAuthenticator authenticator)
        {
            CreateSocket();
            _compressed = new MemoryStream();
            if (UseCompression)
                _decompressor = new DeflateStream(_compressed, CompressionMode.Decompress);
            
            _authenticator = authenticator;
            _gatewayConfig = config;
            eventHandlers = GetEventHandlers();
            operationHandlers = GetOperationHandlers();
            
     //       PrepareSocket();
        }

        public BandwidthStatistics GetStats()
        {
            return _socket.Information.BandwidthStatistics;
        }
        private void CreateSocket()
        {
            _socket?.Dispose();
            _socket = new MessageWebSocket();
            _socket.Control.MessageType = SocketMessageType.Utf8;
            _socket.MessageReceived += HandleMessage;
            _socket.Closed += HandleClosed;
            _dataWriter?.Dispose();
            _dataWriter = new DataWriter(_socket.OutputStream);
        }
        private void HandleClosed(object sender, WebSocketClosedEventArgs args)
        {
            ConnectedSocket = false;
            GatewayClosed?.Invoke(null, args);
            Debug.WriteLine("Gateway closed with code " + args.Code + " and reason \"" + args.Reason + "\"");
        }
        private async void HandleMessage(object sender, MessageWebSocketMessageReceivedEventArgs e)
        {
            try
            {
                if (UseCompression)
                {
                    using (var datastr = e.GetDataStream()?.AsStreamForRead())
                    using (var ms = new MemoryStream())
                    {
                        datastr.CopyTo(ms);
                        ms.Position = 0;
                        byte[] data = new byte[ms.Length];
                        ms.Read(data, 0, (int) ms.Length);
                        int index = 0;
                        int count = data.Length;
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
#if DEBUG
                                string content = await reader.ReadToEndAsync();
                                Debug.WriteLine("<<< " + content);
                                SocketFrame frame = JsonConvert.DeserializeObject<SocketFrame>(content);
                                if(frame.SequenceNumber.HasValue) lastGatewayEventSeq = frame.SequenceNumber.Value;
                                if (operationHandlers.ContainsKey(frame.Operation.GetValueOrDefault()))
                                {
                                    operationHandlers[frame.Operation.GetValueOrDefault()](frame);
                                }

                                if (frame.Type != null && eventHandlers.ContainsKey(frame.Type))
                                {
                                    eventHandlers[frame.Type](frame);
                                }

#else
                                using (JsonReader jsreader = new JsonTextReader(reader))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    SocketFrame frame = serializer.Deserialize<SocketFrame>(jsreader);
                                    if(frame.SequenceNumber.HasValue) lastGatewayEventSeq = frame.SequenceNumber.Value;
                                    if (operationHandlers.ContainsKey(frame.Operation.GetValueOrDefault()))
                                    {
                                        operationHandlers[frame.Operation.GetValueOrDefault()](frame);
                                    }

                                    if (frame.Type != null && eventHandlers.ContainsKey(frame.Type))
                                    {
                                        eventHandlers[frame.Type](frame);
                                    }
                                }
#endif

                            }

                        }
                    }
                }
                else
                {
                    using (var reader = new StreamReader(e.GetDataStream().AsStreamForRead()))
                    using (JsonReader jsreader = new JsonTextReader(reader))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        SocketFrame frame = serializer.Deserialize<SocketFrame>(jsreader);
                        lastGatewayEventSeq = frame.SequenceNumber ?? 0;
                        if (operationHandlers.ContainsKey(frame.Operation.GetValueOrDefault()))
                        {
                            operationHandlers[frame.Operation.GetValueOrDefault()](frame);
                        }

                        if (frame.Type != null && eventHandlers.ContainsKey(frame.Type))
                        {
                            eventHandlers[frame.Type](frame);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ConnectedSocket = false;
                var error = Windows.Networking.Sockets.WebSocketError.GetStatus(exception.HResult);
                if (error == WebErrorStatus.ConnectionAborted)
                {
                    GatewayClosed?.Invoke(null,null);
                }
            }
        }
        public async Task ConnectAsync(string connectionUrl)
        {
            try
            {
                await _socket.ConnectAsync(new Uri(connectionUrl));
                ConnectedSocket = true;
            }
            catch
            {
                ConnectedSocket = false;
            }
        }
        private DataWriter _dataWriter;
        public async Task SendMessageAsync(string message)
        {
#if DEBUG
            Debug.WriteLine(">>> " + message);
#endif
            try
            {
                _dataWriter.WriteString(message);
                await _dataWriter.StoreAsync();
            }
            catch (Exception exception)
            {
                ConnectedSocket = false;
                var error = Windows.Networking.Sockets.WebSocketError.GetStatus(exception.HResult);
                if (error == WebErrorStatus.ConnectionAborted)
                {
                    Debug.WriteLine(error);
                    GatewayClosed?.Invoke(null, null);
                }
            }
        }

        public async Task Search(string query, List<string> guilds, int limit)
        {
            var frame = new SocketFrame()
            {
                Operation = 8,
                Payload = new DiscordAPI.SharedModels.Search
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
        private IDictionary<int, GatewayEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, GatewayEventHandler>
            {
                { OperationCode.Hello.ToInt(), OnHelloReceived },
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
                { EventNames.GUILD_MEMBER_CHUNK, OnGuildMemberChunk },
                { EventNames.PRESENCE_UPDATED, OnPresenceUpdated },
                { EventNames.TYPING_START, OnTypingStarted},
                { EventNames.FRIEND_ADDED, OnRelationShipAdded },
                { EventNames.FRIEND_REMOVED, OnRelationShipRemoved },
                { EventNames.FRIEND_UPDATE, OnRelationShipUpdated },
                { EventNames.USER_NOTE_UPDATED, OnUserNoteUpdated },
                { EventNames.USER_SETTINGS_UPDATED, OnUserSettingsUpdated },
                { EventNames.VOICE_STATE_UPDATED,  OnVoiceStatusUpdated },
                { EventNames.VOICE_SERVER_UPDATED, OnVoiceServerUpdated },
                { EventNames.SESSIONS_REPLACE, OnSessionReplaced },
                { EventNames.CHANNEL_RECIPIENT_ADD, OnChannelRecipientAdded },
                { EventNames.CHANNEL_RECIPIENT_REMOVE, OnChannelRecipientRemoved },
                { EventNames.RESUMED, OnResumeReceived }
            };
        }



       /* private void PrepareSocket()
        {
            _webMessageSocket.MessageReceived += OnSocketMessageReceived;
        }*/
        public static bool UseCompression;
        public async Task ConnectAsync()
        {
            string append = "";
            if (UseCompression)
            {
                append = "&compress=zlib-stream";
            }
            
            await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "6", append));
        }

        // TODO: good chance the socket will be disposed when attempting to resume so yah
        public async Task ResumeAsync()
        {
            //Re-generate the socket
            CreateSocket();
            //Reconnect the socket
            TryResume = true;
            await ConnectAsync();
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
        public async void UpdateStatus(string onlinestatus, int? idleSince, GameBase game)
        {
            if(game != null)
            {
                if (game.Type == 3)
                {
                    game.Type = 0;
                }
                if (game is SharedModels.Game)
                {
                    (game as SharedModels.Game).SessionId = lastReady.SessionId;
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
                Operation = 8,
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
                Operation = 4,
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
            if(channelIds.Length > 193) //This is really, really random, but it's apparently the maximum array size for the op12 event.
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
            if (IsChannelAGuildChannel(gatewayEvent))
            {
                FireEventOnDelegate(gatewayEvent, GuildChannelCreated);
            }
            else
            {
                FireEventOnDelegate(gatewayEvent, DirectMessageChannelCreated);
            }
        }

        private void OnChannelUpdated(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildChannelUpdated);
        }

        private void OnChannelDeleted(SocketFrame gatewayEvent)
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

        private bool IsChannelAGuildChannel(SocketFrame gatewayEvent)
        {
            var dataAsJObject = gatewayEvent.Payload as JObject;
            return dataAsJObject["guild_id"] != null;
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

        private void OnGuildMemberChunk(SocketFrame gatewayEvent)
        {
            FireEventOnDelegate(gatewayEvent, GuildMemberChunk);
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
           // await _webMessageSocket.SendJsonObjectAsync(statusevent);
        }

#endregion
    }
}
