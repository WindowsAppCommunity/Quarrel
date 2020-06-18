using Concentus.Structs;
using DiscordAPI.Models;
using DiscordAPI.Sockets;
using DiscordAPI.Voice.DownstreamEvents;
using DiscordAPI.Voice.UpstreamEvents;
using Newtonsoft.Json;
//using Sodium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using RuntimeComponent;

//Discord DOCs https://discordapp.com/developers/docs/topics/voice-connections


namespace DiscordAPI.Voice
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

        private IReadOnlyDictionary<int, VoiceConnectionEventHandler> operationHandlers;
        private IReadOnlyDictionary<string, VoiceConnectionEventHandler> eventHandlers;

        private Ready lastReady;
        private SocketFrame lastEvent;

        private readonly IWebMessageSocket _webMessageSocket;
        public readonly VoiceState _state;
        private readonly VoiceServerUpdate _voiceServerConfig;


        public event EventHandler<VoiceConnectionEventArgs<Ready>> Ready;
        public event EventHandler<VoiceConnectionEventArgs<VoiceData>> VoiceDataRecieved;
        // TODO: public event EventHandler<VoiceConnectionEventArgs<VoiceData>> VideoDataRecieved;
        public event EventHandler<VoiceConnectionEventArgs<DownstreamEvents.Speak>> Speak;


        private IWebrtcManager _webrtcManager;

        public VoiceConnection()
        {
        }

        public VoiceConnection(VoiceServerUpdate config, VoiceState state, IWebrtcManager webrtcManager)
        {
            //var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
            //mobile = (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");

            _webMessageSocket = new WebMessageSocket();
            _webrtcManager = webrtcManager;
            //_udpSocket = new UDPSocket();
            _state = state;
            _voiceServerConfig = config;


            eventHandlers = GetEventHandlers();
            operationHandlers = GetOperationHandlers();

            PrepareSocket();

            _webrtcManager.IpAndPortObtained += WebrtcManagerOnIpAndPortObtained;
            _webrtcManager.Speaking += WebrtcManagerOnSpeaking;

        }

        private async void WebrtcManagerOnIpAndPortObtained(object sender, Tuple<string, ushort> e)
        {
            var info = new UdpProtocolInfo()
            {
                Address = e.Item1,
                Port = e.Item2,
                Codecs = new List<Codec>()
                {
                    new Codec()
                    {
                        Name = "opus",
                        payloadType = 120,
                        Priority = 1000,
                        Type = "audio",
                        rtxPayloadType = 120,
                    },
                    new Codec()
                    {
                        Name = "VP8",
                        payloadType = 101,
                        Priority = 1000,
                        Type = "video",
                        rtxPayloadType = 102,
                    },
                }.ToArray(),
                Mode = "xsalsa20_poly1305", //"_suffix"
            };

            var payload = new SelectProtocol()
            {
                Protocol = "udp",
                Data = info,
            };

            SocketFrame package = new SocketFrame()
            {
                Operation = (int)OperationCode.SelectProtocol,
                Payload = payload,
                Type = EventNames.SELECT_PROTOCOL,
            };

            await _webMessageSocket.SendJsonObjectAsync(package);
        }

        private void WebrtcManagerOnSpeaking(object sender, bool speaking)
        {
            SendSpeaking(speaking ? 1 : 0);
        }

        private void PrepareSocket()
        {
            _webMessageSocket.MessageReceived += OnSocketMessageReceived;
        }

        public async Task ConnectAsync()
        {
            await _webMessageSocket.ConnectAsync(_voiceServerConfig.GetConnectionUrl("4"));
            IdentifySelfToVoiceConnection();
        }

        private async void IdentifySelfToVoiceConnection()
        {
            var identifyEvent = new SocketFrame
            {
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify.ToInt(),
                Payload = GetIdentityAsync(),
            };

            await _webMessageSocket.SendJsonObjectAsync(identifyEvent);
        }

        public async void SendSpeaking(int speaking)
        {
            DownstreamEvents.Speak Event = new DownstreamEvents.Speak
            {
                Speaking = speaking,
            };
            Speak?.Invoke(this, new VoiceConnectionEventArgs<DownstreamEvents.Speak>(Event));

            var speakingPacket = new SocketFrame
            {
                Operation = OperationCode.Speaking.ToInt(),
                Payload = new UpstreamEvents.Speak()
                {
                    Speaking = speaking,
                    Delay = 0,
                    SSRC = lastReady?.SSRC ?? 0,
                },
            };
            await _webMessageSocket.SendJsonObjectAsync(speakingPacket);
        }

        private IReadOnlyDictionary<int, VoiceConnectionEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, VoiceConnectionEventHandler>
            {
                { OperationCode.Ready.ToInt(), OnReady },
                { OperationCode.Hello.ToInt(), OnHello },
                { OperationCode.SessionDescription.ToInt(), OnSessionDesc },
                { OperationCode.Speaking.ToInt(), OnSpeaking },
            };
        }

        private IReadOnlyDictionary<string, VoiceConnectionEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, VoiceConnectionEventHandler>
            {
                { EventNames.READY, OnReady },
                { EventNames.HEARTBEAT, OnHello},
                { EventNames.SELECT_PROTOCOL, OnSessionDesc },
                { EventNames.SPEAKING, OnSpeaking },
            };
        }

        private Identify GetIdentityAsync()
        {
            return new Identify
            {
                ServerId = _voiceServerConfig.GuildId ?? _voiceServerConfig.ChannelId,
                SessionId = _state.SessionId,
                Token = _voiceServerConfig.Token,
                UserId = _state.UserId,
                Video = false,
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
            BeginHeartbeatAsync(hello.Heartbeatinterval);
        }

        private async void OnReady(SocketFrame Event)
        {
            var ready = Event.GetData<Ready>();
            lastReady = ready;

            FireEventOnDelegate(Event, Ready);

            await _webrtcManager.ConnectAsync(ready.Ip, ready.Port.ToString(), lastReady.SSRC);
        }

        private void OnSessionDesc(SocketFrame Event)
        {
            var Desc = Event.GetData<SessionDescription>();

            _webrtcManager.SetKey(Desc.SecretKey);
        }

        private void OnSpeaking(SocketFrame Event)
        {
            var speak = Event.GetData<DownstreamEvents.Speak>();
            _webrtcManager.SetSpeaking((uint)speak.SSRC, speak.Speaking);
            FireEventOnDelegate(Event, Speak);
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
                    Payload = Math.Round(DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds),
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

