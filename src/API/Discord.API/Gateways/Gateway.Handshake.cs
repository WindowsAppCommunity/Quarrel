// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models.Handshake;
using System;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private static bool _recievedAck = false;

        private bool OnHelloReceived(SocketFrame<Hello> frame)
        {
            SetupGateway(frame.Payload.HeartbeatInterval);
            return true;
        }
        
        private async void SetupGateway(int interval)
        {
            switch (_gatewayStatus)
            {
                case GatewayStatus.Connecting:
                    await IdentifySelfToGateway();
                    _gatewayStatus = GatewayStatus.Connected;
                    break;
                case GatewayStatus.Resuming:
                    await SendResumeRequestAsync();
                    _gatewayStatus = GatewayStatus.Connected;
                    break;
                default:
                    _gatewayStatus = GatewayStatus.Error;
                    return;
            }

            double jitter = (new Random()).NextDouble();
            await Task.Delay((int)(interval * jitter));
            await BeginHeartbeatAsync(interval);
        }

        private bool OnInvalidSession(SocketFrame frame)
        {
            switch (_gatewayStatus)
            {
                case GatewayStatus.InvalidSession:
                    Guard.IsNotNull(_connectionUrl, nameof(_connectionUrl));

                    _ = ConnectAsync(_connectionUrl);
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

        private async Task BeginHeartbeatAsync(int interval)
        {
            while (_gatewayStatus == GatewayStatus.Connected)
            {
                await SendHeartbeatAsync();
                _recievedAck = false;
                await Task.Delay(interval);
                if (!_recievedAck)
                {
                    _gatewayStatus = GatewayStatus.Disconnected;
                    await CloseSocket();
                    await ResumeAsync();
                }
            }
        }

        private async Task SendHeartbeatAsync()
        {
            try
            {
                var frame = new SocketFrame<int>()
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
            var properties = new IdentityProperties()
            {
                OS = "DISCORD-UWP",
                Device = "DISCORD-UWP",
                Browser = "DISCORD-UWP",
                Referrer = string.Empty,
                ReferringDomain = string.Empty,
            };

            var idenity = new Identity()
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
                Payload = idenity,
            };

            await SendMessageAsync(payload);
        }

        private async Task SendResumeRequestAsync()
        {
            Guard.IsNotNull(_sessionId, nameof(_sessionId));
            
            var request = new SocketFrame<GatewayResume>()
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
