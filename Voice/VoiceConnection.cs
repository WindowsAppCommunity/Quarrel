using Concentus;
using Concentus.Structs;
using Discord_UWP.Authentication;
using Discord_UWP.SharedModels;
using Discord_UWP.Sockets;
using Discord_UWP.Voice.DownstreamEvents;
using Discord_UWP.Voice.UpstreamEvents;
using Newtonsoft.Json;
//using Sodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

//Discord DOCs https://discordapp.com/developers/docs/topics/voice-connections


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
        private SocketFrame? lastEvent;

        private readonly IWebMessageSocket _webMessageSocket;
        private readonly UDPSocket _udpSocket;
        private readonly VoiceState _state;
        private readonly VoiceServerUpdate _voiceServerConfig;
        private byte[] _nonce = new byte[24];
        private byte[] _encrypted = new byte[3840];
        private byte[] _unencrypted = new byte[3840];

        private byte[] secretkey;

        public event EventHandler<VoiceConnectionEventArgs<Ready>> Ready;
        public event EventHandler<VoiceConnectionEventArgs<VoiceData>> VoiceDataRecieved;

        public VoiceConnection()
        {

        }

        public VoiceConnection(VoiceServerUpdate config, VoiceState state)
        {
            _webMessageSocket = new WebMessageSocket();
            _udpSocket = new UDPSocket();
            _state = state;
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
            await _webMessageSocket.ConnectAsync(_voiceServerConfig.GetConnectionUrl("3"));
            IdentifySelfToVoiceConnection();
        }

        private async Task ConnectUDPAsync(string Ip, string Port)
        {
            await _udpSocket.ConnectAsync(Ip, Port);
        }

        private async void IdentifySelfToVoiceConnection()
        {
            var identifyEvent = new SocketFrame
            {
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify.ToInt(),
                Payload = GetIdentityAsync()
            };

            await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        private async void SendSelectProtocol()
        {
            _udpSocket.MessageReceived += IpDiscover;
            await _udpSocket.SendDiscovery(lastReady.Value.SSRC);
        }

        public void SendSpeaking(bool speaking)
        {
            
        }

        public void SendVoiceHeader()
        {
            //_rtpHeader[0] = 0x80;
            //_rtpHeader[1] = 0x78;

            //StreamEncryption.EncryptXSalsa20(new byte[12], new byte[12], secretkey);
        }

        public void SendVoiceData()
        {

        }

        async void IpDiscover(object sender, PacketReceivedEventArgs args)
        {
            var packet = (byte[])args.Message;
            string ip = "";
            int port = 0;
            try
            {
                ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');
                port = (packet[69] << 8) | packet[68];
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }

            var info = new UdpProtocolInfo()
            {
                Address = ip,
                Port = port,
                Mode = "xsalsa20_poly1305"
            };

            var payload = new SelectProtocol()
            {
                Protocol = "udp",
                Data = info
            };

            SocketFrame package = new SocketFrame()
            {
                Operation = 1,
                Payload = payload,
                Type = EventNames.SELECT_PROTOCOL
            };

            await _webMessageSocket.SendJsonObjectAsync(package);

            _udpSocket.MessageReceived -= IpDiscover;
            _udpSocket.MessageReceived += processVoicePacket;
        }

        private IDictionary<int, VoiceConnectionEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, VoiceConnectionEventHandler>
            {
                { OperationCode.Ready.ToInt(), OnReady },
                { OperationCode.Hello.ToInt(), OnHello },
                { OperationCode.SessionDescription.ToInt(), OnSessionDesc }
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
                GuildId = _voiceServerConfig.GuildId,
                SessionId = _state.SessionId,
                Token = _voiceServerConfig.Token,
                UserId = _state.UserId
            };
        }

        private void OnSocketMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var gatewayEvent = JsonConvert.DeserializeObject<SocketFrame>(args.Message);
            lastEvent = gatewayEvent;

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

        private void OnHello(SocketFrame Event)
        {
            var hello = Event.GetData<Hello>();
            BeginHeartbeatAsync(hello.Heartbeatinterval * .75); //The *.75 is due to a serverside bug
        }

        private async void OnReady(SocketFrame Event)
        {
            var ready = Event.GetData<Ready>();
            lastReady = ready;

            FireEventOnDelegate(Event, Ready);

            await ConnectUDPAsync(ready.Ip, ready.Port.ToString());
            SendSelectProtocol();
        }

        private void OnSessionDesc(SocketFrame Event)
        {
            var Desc = Event.GetData<SessionDescription>();
            secretkey = Desc.SecretKey;
        }

        private void processVoicePacket(object sender, PacketReceivedEventArgs e)
        {
            try
            {
                var packet = (byte[])e.Message;
                Buffer.BlockCopy(packet, 0, _nonce, 0, 12);
                Buffer.BlockCopy(packet, 12, _encrypted, 0, packet.Length-12);

                //_unencrypted = StreamEncryption.DecryptXSalsa20(_encrypted, _nonce, secretkey);

                OpusDecoder decoder = new OpusDecoder(48000, 2);
                //Framesize is wrong
                int framesize = 20 * 48 * 2 * 2; //20 ms * 48 samples per ms * 2 channels * 2 bytes per sample
                float[] output = new float[framesize]; // framesize 
                int samples = decoder.Decode(_unencrypted, 0, _unencrypted.Length, output, 0, framesize);

                //TODO: CPPReference
                

                VoiceDataRecieved?.Invoke(null, new VoiceConnectionEventArgs<VoiceData>(new VoiceData() { data = output, samples = (uint)samples }));
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.Message);
                //App.NavigateToBugReport(exception);
            }
        }

        #endregion

        private void FireEventOnDelegate<TEventData>(SocketFrame gatewayEvent, EventHandler<VoiceConnectionEventArgs<TEventData>> eventHandler)
        {
            var eventArgs = new VoiceConnectionEventArgs<TEventData>(gatewayEvent.GetData<TEventData>());
            eventHandler?.Invoke(this, eventArgs);
        }

        private async void BeginHeartbeatAsync(double interval)
        {
            while (true)
            {
                await Task.Delay(Convert.ToInt32(interval));
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
                    Payload = lastEvent?.SequenceNumber ?? 0
                };

                await _webMessageSocket.SendJsonObjectAsync(heartbeatEvent);
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }
        }
    }
}
