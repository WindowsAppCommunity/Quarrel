// Quarrel © 2022

using Discord.API.Gateways.Models.Handshake;
using Discord.API.Voice.Models.Handshake;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private bool _receivedAck;

        public async Task SelectProtocol(string ip, int port)
        {
            var payload = new SelectProtocol<UdpProtocolInfo>
            {
                Codecs = new Codec[]
                {
                    new()
                    {
                        Name = "opus",
                        PayloadType = 120,
                        Priority = 1000,
                        Type = "audio",
                    },
                    new()
                    {

                        Name = "H264",
                        PayloadType = 101,
                        Priority = 3000,
                        RtxPayloadType = 102,
                        Type = "video",
                        Decode = true
                    },
                    new()
                    {

                        Name = "VP8",
                        PayloadType = 103,
                        Priority = 2000,
                        RtxPayloadType = 104,
                        Type = "video",
                        Decode = true
                    },
                    new()
                    {

                        Name = "VP9",
                        PayloadType = 105,
                        Priority = 3000,
                        RtxPayloadType = 106,
                        Type = "video",
                        Decode = true
                    }
                },
                Data = new UdpProtocolInfo
                {
                    Address = ip,
                    Port = port,
                    Mode = "xsalsa20_poly1305"
                },
                Address = ip,
                Port = port,
                Mode = "xsalsa20_poly1305",
                Protocol = "udp",
            };

            await SendMessageAsync(VoiceOperation.SelectProtocol, payload);
        }

        public async Task SendVideo(uint audioSSRC, VoiceReady.Stream[] streams)
        {
            await SendMessageAsync(VoiceOperation.Video, new Video
            {
                AudioSSRC = audioSSRC,
                RtxSSRC = 0,
                VideoSSRC = 0,
                Streams = streams.Select(x => new Video.VideoStream
                {
                    Active = x.Active,
                    MaxBitrate = 720 * 1280 * 4,
                    MaxFramerate = 30,
                    MaxResolution = new Video.VideoStream.Resolution
                    {
                        Height = 720,
                        Width = 1280,
                        Type = "fixed"
                    },
                    Quality = x.Quality,
                    Rid = x.Rid,
                    RtxSSRC = x.RtxSSRC,
                    SSRC = x.SSRC,
                    Type = "Video"
                }).ToArray()
            });
        }

        private bool OnHeartbeatAck()
        {
            _receivedAck = true;
            return true;
        }

        private async Task BeginHeartbeatAsync(int interval, CancellationToken token)
        {
            double jitter = new Random().NextDouble();
            await Task.Delay((int)(interval * jitter), token);
            while (!token.IsCancellationRequested)
            {
                await SendHeartbeatAsync();
                _receivedAck = false;
                await Task.Delay(interval, token);
                if (!token.IsCancellationRequested && !_receivedAck)
                {
                    VoiceConnectionStatus = VoiceConnectionStatus.Disconnected;
                    //await ResumeAsync();
                }
            }
        }

        protected async Task SendHeartbeatAsync()
        {
            try
            {
                int nonce = (int)Math.Round(DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                await SendMessageAsync(VoiceOperation.Heartbeat, nonce);
            }
            catch
            {
            }
        }

        private bool OnHello(VoiceSocketFrame<VoiceHello> frame)
        {
            return SetupVoiceConnection(frame.Payload.HeartbeatInterval);
        }

        private bool OnReady(VoiceSocketFrame<VoiceReady> frame)
        {
            var ready = frame.Payload;
            _ssrc = ready.SSRC;

            return FireEvent(frame, Ready);
        }

        private bool SetupVoiceConnection(int interval)
        {
            switch (VoiceConnectionStatus)
            {
                case VoiceConnectionStatus.Reconnecting:
                case VoiceConnectionStatus.Connecting:
                case VoiceConnectionStatus.Resuming:
                    VoiceConnectionStatus = VoiceConnectionStatus.Connected;
                    break;
                default:
                    VoiceConnectionStatus = VoiceConnectionStatus.Error;
                    return false;
            }

            _ = BeginHeartbeatAsync(interval, _socket!.Token);
            return true;
        }

        public async Task IdentifySelfToVoiceConnection(ulong serverId, string sessionId, string token, ulong userId, bool video, VoiceIdentity.VoiceIdentityStream[]? streams = null)
        {
            var identity = new VoiceIdentity
            {
                ServerId = serverId,
                SessionId = sessionId,
                Token = token,
                UserId = userId,
                Video = video,
                Streams = streams
            };

            await SendMessageAsync(VoiceOperation.Identify, identity);
        }
    }
}
