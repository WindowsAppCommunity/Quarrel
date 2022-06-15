// Quarrel © 2022

using Discord.API.Sockets;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;
        private DiscordSocketClient<GatewaySocketFrame>? _socket;

        /// <summary>
        /// Sets up a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task Connect(string token)
        {
            _token = token;
            GatewayStatus = GatewayStatus == GatewayStatus.Initialized ? GatewayStatus.Connecting : GatewayStatus.Reconnecting;
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "9", "&compress=zlib-stream"));
        }
        
        public async Task ConnectAsync(string url)
        {
            _connectionUrl = url;
            _socket = new DiscordSocketClient<GatewaySocketFrame>(_serializeOptions, _deserializeOptions, HandleMessage, HandleError, UnhandledMessageEncountered);
            await _socket.ConnectAsync(url);
        }

        /// <summary>
        /// Resumes a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task ResumeAsync()
        {
            await _socket!.CloseSocket((WebSocketCloseStatus)4000);
            GatewayStatus = GatewayStatus.Resuming;
            await ConnectAsync();
        }

        private async Task ReconnectAsync()
        {
            await _socket!.CloseSocket((WebSocketCloseStatus)4000);
            await ConnectAsync(_connectionUrl!);
        }

        private async Task SendMessageAsync<T>(GatewayOperation op, T payload)
            => await SendMessageAsync(op, null, payload);

        private async Task SendMessageAsync<T>(GatewayOperation op, GatewayEvent? e, T payload)
        {
            var frame = new GatewaySocketFrame<T>
            {
                Operation = op,
                Event = e,
                Payload = payload,
            };

            await _socket!.SendMessageAsync(frame);
        }

        private void HandleMessage(GatewaySocketFrame frame)
        {
            if (frame.SequenceNumber.HasValue)
            {
                _lastEventSequenceNumber = frame.SequenceNumber.Value;
            }

            ProcessEvents(frame);
        }

        private void HandleError(WebSocketCloseStatus? status)
        {
            switch (status)
            {
                case (WebSocketCloseStatus)4000:
                case (WebSocketCloseStatus)4001:
                case (WebSocketCloseStatus)4002:
                case (WebSocketCloseStatus)4003:
                case (WebSocketCloseStatus)4005:
                case (WebSocketCloseStatus)4007:
                case (WebSocketCloseStatus)4008:
                case (WebSocketCloseStatus)4009:
                    GatewayStatus = GatewayStatus.Reconnecting;
                    _socket = null;
                    _ = ConnectAsync();
                    return;

                case (WebSocketCloseStatus)4004:
                default:
                    GatewayStatus = GatewayStatus.Disconnected;
                    _socket = null;
                    return;

            }
        }
    }
}