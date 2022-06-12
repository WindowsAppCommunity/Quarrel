// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models.Handshake;
using System;
using System.Threading.Tasks;
using Discord.API.Sockets;
using System.Threading;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private static bool _recievedAck = false;

        private bool OnHelloReceived(GatewaySocketFrame<Hello> frame)
        {
            _ = SetupGateway(frame.Payload.HeartbeatInterval);
            return true;
        }

        private async Task SetupGateway(int interval)
        {
            switch (GatewayStatus)
            {
                case GatewayStatus.Reconnecting:
                case GatewayStatus.Connecting:
                    await IdentifySelfToGateway();
                    GatewayStatus = GatewayStatus.Connected;
                    break;
                case GatewayStatus.Resuming:
                    await SendResumeRequestAsync();
                    GatewayStatus = GatewayStatus.Connected;
                    break;
                default:
                    GatewayStatus = GatewayStatus.Error;
                    return;
            }

            double jitter = (new Random()).NextDouble();
            await Task.Delay((int)(interval * jitter));
            _ = BeginHeartbeatAsync(interval, _socket!.Token);
        }

        private bool OnInvalidSession(GatewaySocketFrame frame)
        {
            switch (GatewayStatus)
            {
                case GatewayStatus.InvalidSession:
                    _ = ReconnectAsync();
                    break;
                case GatewayStatus.Reconnecting:
                    FireEvent(frame, InvalidSession);
                    break;
            }

            return true;
        }

        private bool OnHeartbeatAck()
        {
            _recievedAck = true;
            return true;
        }

        private async Task BeginHeartbeatAsync(int interval, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await SendHeartbeatAsync();
                _recievedAck = false;
                await Task.Delay(interval, token);
                if (!token.IsCancellationRequested && !_recievedAck)
                {
                    GatewayStatus = GatewayStatus.Disconnected;
                    await ResumeAsync();
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var frame = new GatewaySocketFrame<int>()
                {
                    Operation = GatewayOperation.Heartbeat,
                    Payload = _lastEventSequenceNumber
                };

                await SendMessageAsync(frame);
            }
            catch
            {
            }
        }

        private async Task IdentifySelfToGateway()
        {
            Guard.IsNotNull(_token, nameof(_token));

            var properties = new IdentityProperties()
            {
                OS = "DISCORD-UWP",
                Device = "DISCORD-UWP",
                Browser = "DISCORD-UWP",
                Referrer = string.Empty,
                ReferringDomain = string.Empty,
            };

            var identity = new Identity()
            {
                Token = _token,
                Compress = false,
                LargeThreshold = 250,
                Properties = properties,
            };

            var payload = new GatewaySocketFrame<Identity>()
            {
                Event = GatewayEvent.IDENTIFY,
                Operation = GatewayOperation.Identify,
                Payload = identity,
            };

            await SendMessageAsync(payload);
        }

        private async Task SendResumeRequestAsync()
        {
            Guard.IsNotNull(_sessionId, nameof(_sessionId));
            Guard.IsNotNull(_token, nameof(_token));

            var request = new GatewaySocketFrame<GatewayResume>()
            {
                Operation = GatewayOperation.Resume,
                Payload = {
                    Token = _token,
                    SessionID = _sessionId,
                    LastSequenceNumberReceived = _lastEventSequenceNumber,
                },
            };

            await SendMessageAsync(request);
        }
    }
}