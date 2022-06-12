// Quarrel © 2022

using Discord.API.Sockets;
using System;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace Discord.API.Voice
{
    internal partial class VoiceConnection
    {
        private readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;
        private DiscordSocketClient<VoiceSocketFrame>? _socket;

        /// <summary>
        /// Sets up a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task Connect(string token)
        {
            VoiceConnectionStatus = VoiceConnectionStatus == VoiceConnectionStatus.Initialized ? VoiceConnectionStatus.Connecting : VoiceConnectionStatus.Reconnecting;
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            // TODO: get connection url.
            string connectionUrl = string.Empty;
            await ConnectAsync(connectionUrl);
        }

        public async Task ConnectAsync(string url)
        {
            _connectionUrl = url;
            _socket = new DiscordSocketClient<VoiceSocketFrame>(_serializeOptions, _deserializeOptions, HandleMessage, HandleError, UnhandledMessageEncountered);
            await _socket.ConnectAsync(url);
        }

        private async Task SendMessageAsync<T>(VoiceOperation op, T payload)
            => await SendMessageAsync(op, null, payload);

        private async Task SendMessageAsync<T>(VoiceOperation op, VoiceEvent? e, T payload)
        {
            var frame = new VoiceSocketFrame<T>
            {
                Operation = op,
                Event = e,
                Payload = payload,
            };

            await _socket!.SendMessageAsync(frame);
        }

        private void HandleMessage(VoiceSocketFrame frame)
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
                default:
                    VoiceConnectionStatus = VoiceConnectionStatus.Disconnected;
                    _socket = null;
                    return;

            }
        }
    }
}
