// Quarrel © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways.Models.Handshake;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
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
                    FireEventOnDelegate(frame, InvalidSession);
                    break;
            }

            return true;
        }

        private async Task BeginHeartbeatAsync(int interval)
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
                var frame = new SocketFrame<int>()
                {
                    Operation = OperationCode.Heartbeat,
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
                Type = EventNames.IDENTIFY,
                Operation = OperationCode.Identify,
                Payload = idenity,
            };

            await SendMessageAsync(payload);
        }

        private async Task SendResumeRequestAsync()
        {
            Guard.IsNotNull(_sessionId, nameof(_sessionId));

            var payload = new GatewayResume()
            {
                Token = _token,
                SessionID = _sessionId,
                LastSequenceNumberReceived = _lastEventSequenceNumber,
            };

            var request = new SocketFrame<GatewayResume>()
            {
                Operation = OperationCode.Resume,
                Payload = payload,
            };

            await SendMessageAsync(request);
        }
    }
}
