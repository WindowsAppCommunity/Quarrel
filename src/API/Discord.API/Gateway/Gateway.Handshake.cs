// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateway.Models.Handshake;
using System.Threading.Tasks;

namespace Discord.API.Gateway
{
    internal partial class Gateway
    {
        private async void OnHelloReceived(SocketFrame frame)
        {
            if (_gatewayStatus == GatewayStatus.Connecting)
            {
                await IdentifySelfToGateway();
                _gatewayStatus = GatewayStatus.Connected;
            }
            else if (_gatewayStatus == GatewayStatus.Resuming)
            {
                await SendResumeRequestAsync();
                _gatewayStatus = GatewayStatus.Connected;
            }
            else
            {
                _gatewayStatus = GatewayStatus.Error;
                return;
            }

            Hello? data = frame.GetData<Hello>();
            Guard.IsNotNull(data, nameof(data));
            await BeginHeartbeatAsync(data.HeartbeatInterval);
        }

        private async void OnInvalidSession(SocketFrame frame)
        {
            if (_gatewayStatus == GatewayStatus.InvalidSession)
            {
                Guard.IsNotNull(_connectionUrl, nameof(_connectionUrl));

                _ = await ConnectAsync(_connectionUrl);
            }
            else if (_gatewayStatus == GatewayStatus.Reconnecting)
            {
                FireEventOnDelegate(frame, InvalidSession);
            }
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
                var frame = new SocketFrame()
                {
                    Operation = OperationCode.Heartbeat,
                    SequenceNumber = _lastEventSequenceNumber
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
                OS = "Windows",
                Device = "Quarrel",
                Browser = "Quarrel",
                Referrer = "",
                ReferringDomain = ""
            };

            var idenity = new Identity()
            {
                Token = _token,
                Compress = true,
                LargeThreshold = 250,
                Properties = properties,
            };

            var payload = new SocketFrame()
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

            var request = new SocketFrame()
            {
                Operation = OperationCode.Resume,
                Payload = payload,
            };

            await SendMessageAsync(request);
        }
    }
}
