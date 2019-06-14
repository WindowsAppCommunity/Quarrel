using Concentus.Structs;
using Newtonsoft.Json;
//using Sodium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DiscordAPI.SharedModels;
using Quarrel.Sockets;
using Quarrel.Voice.DownstreamEvents;
using Quarrel.Voice.UpstreamEvents;
using RuntimeComponent;

//Discord DOCs https://discordapp.com/developers/docs/topics/voice-connections


namespace Quarrel.Voice
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

        private Ready lastReady;
        private SocketFrame lastEvent;

        private readonly IWebMessageSocket _webMessageSocket;
        private readonly UDPSocket _udpSocket;
        private readonly VoiceState _state;
        private readonly VoiceServerUpdate _voiceServerConfig;

        //private float[] partialFrame = null;
        byte[] buffer = new byte[FrameBytes];

        private OpusEncoder encoder = new OpusEncoder(48000, 2, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP);
        private OpusDecoder decoder = new OpusDecoder(48000, 2);

        private ushort sequence = 0;

        private const int framesize = 20 * 48 * 2; //20 ms * 48 samples per ms * 2 bytes per sample

        public const int SamplingRate = 48000;
        public const int Channels = 2;
        public const int FrameMillis = 20;

        public const int VideoDeviceBitrate = 600000;
        public const int VideoDeviceMinBitrate = 150000;
        public const int VideoDeviceMaxBitrate = 2500000;
        public const int VideoDeviceWidth = 1280;
        public const int VideoDeviceHeight = 720;
        public const int VideoFrameRate = 30;

        public const int DesktopBitrate = 600000;
        public const int DesktopMinBitrate = 150000;
        public const int DesktopMaxBitrate = 2500000;
        public const int DesktopWidth = 1280;
        public const int DesktopHeight = 720;
        public const int DesktopFrameRate = 30;

        public const int SampleBytes = sizeof(float) * Channels;

        public const int FrameSamplesPerChannel = SamplingRate / 1000 * FrameMillis;
        public const int FrameSamples = FrameSamplesPerChannel * Channels;
        public const int FrameBytes = FrameSamplesPerChannel * SampleBytes;

        private float[] output = new float[framesize*2];

        private byte[] secretkey;

        private uint timestamp;

        public event EventHandler<VoiceConnectionEventArgs<Ready>> Ready;
        public event EventHandler<VoiceConnectionEventArgs<VoiceData>> VoiceDataRecieved;
        public event EventHandler<VoiceConnectionEventArgs<VoiceData>> VideoDataRecieved;
        public event EventHandler<VoiceConnectionEventArgs<DownstreamEvents.Speak>> Speak;

        public VoiceConnection()
        {

        }

        public VoiceConnection(VoiceServerUpdate config, VoiceState state)
        {
            //var qualifiers = ResourceContext.GetForCurrentView().QualifierValues;
            //mobile = (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");

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
            await _webMessageSocket.ConnectAsync(_voiceServerConfig.GetConnectionUrl("4"));
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
            await _udpSocket.SendDiscovery(lastReady.SSRC);
        }

        public async void SendSilence()
        {
            if (lastReady != null)
            {
                byte[] opus = new byte[31];
                byte[] nonce = makeHeader();
                Buffer.BlockCopy(nonce, 0, opus, 0, 12);
                opus[12] = 0xF8;
                opus[13] = 0xFF;
                opus[14] = 0xFE;
                Cypher.encrypt(opus, 12, 3, opus, 12, nonce, secretkey);
                await _udpSocket.SendBytesAsync(opus);
            }
        }

        public async void SendSpeaking(bool speaking)
        {
            DownstreamEvents.Speak Event = new DownstreamEvents.Speak
            {
                Speaking = speaking
            };
            Speak?.Invoke(this, new VoiceConnectionEventArgs<DownstreamEvents.Speak>(Event));
            if (speaking == false)
            {
                SendSilence();
            }

            var speakingPacket = new SocketFrame
            {
                Operation = OperationCode.Speaking.ToInt(),
                Payload = new UpstreamEvents.Speak()
                {
                    Speaking = speaking,
                    Delay = 0,
                    SSRC = lastReady != null ? lastReady.SSRC : 0
                }
            };
            await _webMessageSocket.SendJsonObjectAsync(speakingPacket);
        }

        byte[] makeHeader()
        {
            byte[] header = new byte[24];
            if (lastReady != null)
            {
                sequence = unchecked ((ushort) (sequence + 1));
                header[0] = 0x80; //No extension
                header[1] = 0x78;

                //byte[] seq = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(sequence));
                if (BitConverter.IsLittleEndian)
                {
                    header[2] = (byte)(sequence >> 8);
                    header[3] = (byte)(sequence >> 0);
                } else
                {
                    header[2] = (byte)(sequence >> 0);
                    header[3] = (byte)(sequence >> 8);
                }



                byte[] time = BitConverter.GetBytes(timestamp);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(time);
                }
                header[4] = time[0];
                header[5] = time[1];
                header[6] = time[2];
                header[7] = time[3];

                byte[] SSRCBytes = BitConverter.GetBytes(lastReady.SSRC);

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(SSRCBytes);
                }
                header[8] = SSRCBytes[0];
                header[9] = SSRCBytes[1];
                header[10] = SSRCBytes[2];
                header[11] = SSRCBytes[3];
            }
            return header;
        }

        public async void SendVoiceData(float[] frame)
        {
            if (lastReady != null && frame.Length == 1920 && secretkey != null)
            {
                int encodedSize = encoder.Encode(frame, 0, FrameSamplesPerChannel, buffer, 0, FrameBytes);

                byte[] opus = new byte[encodedSize + 12 + 16];
                byte[] nonce = makeHeader();
                Buffer.BlockCopy(nonce, 0, opus, 0, 12);
                Buffer.BlockCopy(buffer, 0, opus, 12, encodedSize);
                timestamp = unchecked(timestamp + 960);
                Cypher.encrypt(opus, 12, encodedSize, opus, 12, nonce, secretkey);
                await _udpSocket.SendBytesAsync(opus);
            }
        }

        async void IpDiscover(object sender, PacketReceivedEventArgs args)
        {
            var packet = (byte[])args.Message;
            string ip = "";
            int port = 0;
            try
            {
                ip = Encoding.UTF8.GetString(packet, 4, 70 - 6).TrimEnd('\0');
                port = (packet[68] << 8) | packet[69];
            }
            catch /*(Exception exception)*/
            {
                //App.NavigateToBugReport(exception);
            }

            var info = new UdpProtocolInfo()
            {
                Address = ip,
                Port = port,
                Codecs = new List<Codec>()
                {
                    new Codec()
                    {
                        Name = "opus",
                        payloadType = 120,
                        Priority = 1000,
                        Type = "audio",
                        rtxPayloadType = 120
                    },
                    new Codec()
                    {
                        Name = "VP8",
                        payloadType = 101,
                        Priority = 1000,
                        Type = "video",
                        rtxPayloadType = 102
                    }
                }.ToArray(),
                Mode = "xsalsa20_poly1305" //"_suffix"
            };

            var payload = new SelectProtocol()
            {
                Protocol = "udp",
                Data = info
            };

            SocketFrame package = new SocketFrame()
            {
                Operation = (int)OperationCode.SelectProtocol,
                Payload = payload,
                Type = EventNames.SELECT_PROTOCOL
            };

            _udpSocket.MessageReceived -= IpDiscover;
            _udpSocket.MessageReceived += processUdpPacket;

            await _webMessageSocket.SendJsonObjectAsync(package);
        }

        private IDictionary<int, VoiceConnectionEventHandler> GetOperationHandlers()
        {
            return new Dictionary<int, VoiceConnectionEventHandler>
            {
                { OperationCode.Ready.ToInt(), OnReady },
                { OperationCode.Hello.ToInt(), OnHello },
                { OperationCode.SessionDescription.ToInt(), OnSessionDesc },
                {OperationCode.Speaking.ToInt(), OnSpeaking }
            };
        }

        private IDictionary<string, VoiceConnectionEventHandler> GetEventHandlers()
        {
            return new Dictionary<string, VoiceConnectionEventHandler>
            {
                { EventNames.READY, OnReady },
                { EventNames.HEARTBEAT, OnHello},
                { EventNames.SELECT_PROTOCOL, OnSessionDesc },
                { EventNames.SPEAKING, OnSpeaking }
            };
        }

        private Identify GetIdentityAsync()
        {
            return new Identify
            {
                GuildId = _voiceServerConfig.GuildId,
                SessionId = _state.SessionId,
                Token = _voiceServerConfig.Token,
                UserId = _state.UserId,
                Video = false
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


            //Needs to speak 100 silent frames to get first listen packet
            SendSpeaking(true);
            for (int i = 0; i < 100; i++)
            {
                SendSilence();
            }
            SendSpeaking(false);
        }

        private void OnSpeaking(SocketFrame Event)
        {
           var Speaking = Event.GetData<DownstreamEvents.Speak>();
            FireEventOnDelegate(Event, Speak);
        }

        private void processUdpPacket(object sender, PacketReceivedEventArgs e)
        {
            if (secretkey != null)
            {
                byte[] packet = (byte[])e.Message;
                byte[] nonce = new byte[24];
                byte[] data;
                Buffer.BlockCopy(packet, 0, nonce, 0, 12);
                data = new byte[packet.Length - 12 - 16];

                int payloadType = packet[1];
                switch (payloadType)
                {
                    case 120:
                        //Opus Audio
                        if (data.Length != Cypher.decrypt(packet, 12, packet.Length - 12, data, 0, nonce, secretkey))
                        {
                            throw new Exception("Conflicting sizes"); //Conflicting sizes
                        }
                        processVoicePacket(packet, data);
                        break;
                    case 101:
                        //VP8 Video
                        if (data.Length != Cypher.decrypt(packet, 12, packet.Length - 12, data, 0, nonce, secretkey))
                        {
                            throw new Exception("Conflicting sizes"); //Conflicting sizes
                        }
                        processVP8Packet(packet, data);
                        break;
                    case 102:
                        //TODO: RTX Video
                        Debug.WriteLine($"RTX payload: {payloadType}");
                        break;
                    default:
                        Debug.WriteLine($"Unknown payload Type: {payloadType}");
                        break;
                }
            }
        }

        private void processVoicePacket(byte[] packet, byte[] data)
        {
            try
            {
                int headerSize = GetHeaderSize(packet, data);
                int samples = decoder.Decode(data, headerSize, data.Length - headerSize, output, 0, framesize);

                VoiceDataRecieved?.Invoke(null, new VoiceConnectionEventArgs<VoiceData>(new VoiceData() { data = output, samples = (uint)samples }));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }
        }

        private void processVP8Packet(byte[] packet, byte[] data)
        {
            try
            {
             //   int headerSize = GetHeaderSize(packet, data);
              //  int samples = decoder.Decode(data, headerSize, data.Length - headerSize, output, 0, framesize);

             //   VideoDataRecieved?.Invoke(null, new VoiceConnectionEventArgs<VoiceData>(new VoiceData() { data = output, samples = (uint)samples }));
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
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

        public static int GetHeaderSize(byte[] header, byte[] buffer)
        {
            byte headerByte = header[0];
            bool extension = (headerByte & 0b0001_0000) != 0;
            int csics = (headerByte & 0b0000_1111) >> 4;

            if (!extension)
                return csics * 4;

            int extensionOffset = csics * 4;
            int extensionLength =
                (buffer[extensionOffset + 2] << 8) |
                (buffer[extensionOffset + 3]);
            return extensionOffset + 4 + (extensionLength * 4);
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var heartbeatEvent = new SocketFrame
                {
                    Operation = OperationCode.Heartbeat.ToInt(),
                    Payload = Math.Round(DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)
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
