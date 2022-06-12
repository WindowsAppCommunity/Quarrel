// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Sockets;
using System;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private bool OnHelloReceived(SocketFrame<Hello> frame)
        {
            _ = SetupGateway(frame.Payload.HeartbeatInterval);
            return true;
        }
        
        private async Task SetupGateway(int interval)
        {
            switch (ConnectionStatus)
            {
                case ConnectionStatus.Reconnecting:
                case ConnectionStatus.Connecting:
                    await IdentifySelfToGateway();
                    ConnectionStatus = ConnectionStatus.Connected;
                    break;
                case ConnectionStatus.Resuming:
                    await SendResumeRequestAsync();
                    ConnectionStatus = ConnectionStatus.Connected;
                    break;
                default:
                    ConnectionStatus = ConnectionStatus.Error;
                    return;
            }

            double jitter = new Random().NextDouble();
            await Task.Delay((int)(interval * jitter));
            _ = BeginHeartbeatAsync(interval);
        }

        private bool OnInvalidSession(SocketFrame frame)
        {
            switch (ConnectionStatus)
            {
                case ConnectionStatus.InvalidSession:
                    _ = ReconnectAsync();
                    break;
                case ConnectionStatus.Reconnecting:
                    FireEvent(frame, InvalidSession);
                    break;
            }

            return true;
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

            var payload = new SocketFrame<Identity>()
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
            
            var request = new SocketFrame<GatewayResume>()
            {
                Operation = GatewayOperation.Resume,
                Payload = {
                    Token = _token,
                    SessionID = _sessionId,
                    LastSequenceNumberReceived = LastEventSequenceNumber,
                },
            };

            await SendMessageAsync(request);
        }
    }
}
