// Quarrel © 2022

using Discord.API.Voice.Models.Handshake;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private bool _receivedAck;

        public async Task SelectProtocol(VoiceReady ready)
        {
            var protocolInfo = new UdpProtocolInfo
            {
                Address = ready.IP,
                Port = ready.Port,
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
                        Name = "VP8",
                        PayloadType = 101,
                        Priority = 1000,
                        Type = "video"
                    }
                },
                Mode = "xsalsa20_poly1305"
            };
            
            var payload = new SelectProtocol<UdpProtocolInfo>
            {
                Protocol = "udp",
                Data = protocolInfo,
            };

            await SendMessageAsync(VoiceOperation.SelectProtocol, payload);
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

        private async Task IdentifySelfToVoiceConnection()
        {
            var identity = new VoiceIdentity
            {
                ServerId = _voiceConfig.GuildId ?? _voiceConfig.ChannelId,
                SessionId = _state.SessionId,
                Token = _voiceConfig.Token,
                UserId = _state.UserId,
                Video = false,
            };

            await SendMessageAsync(VoiceOperation.Identify, identity);
        }
    }
}
