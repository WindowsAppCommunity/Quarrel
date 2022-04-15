// Quarrel © 2022

using System;
using Discord.API.JsonConverters;
using Discord.API.Models.Json.Gateway;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

namespace Discord.API.Gateways
{
    internal partial class Gateway
    {
        private delegate void GatewayEventHandler(SocketFrame gatewayEvent);

        private readonly GatewayConfig _gatewayConfig;
        private readonly string _token;

        private GatewayStatus _gatewayStatus;
        private string? _connectionUrl;
        private string? _sessionId;
        private int _lastEventSequenceNumber;

        public Gateway(GatewayConfig config, string token)
        {
            _gatewayConfig = config;
            _token = token;

            _socket = CreateSocket();
            SetupCompression();

            _gatewayStatus = GatewayStatus.Initialized;

            _serialiseOptions = new JsonSerializerOptions();
            _serialiseOptions.AddContext<JsonModelsContext>();

            _deserialiseOptions = new JsonSerializerOptions { Converters = { new SocketFrameConverter() } };
        }

        /// <summary>
        /// Sets up a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task ConnectAsync()
        {
            _gatewayStatus = GatewayStatus.Connecting;
            await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "9", "&compress=zlib-stream"));
        }

        /// <summary>
        /// Resumes a connection to the gateway.
        /// </summary>
        /// <exception cref="Exception">An exception will be thrown when connection fails, but not when the handshake fails.</exception>
        public async Task ResumeAsync()
        {
            _gatewayStatus = GatewayStatus.Resuming;
            _socket = CreateSocket();
            await ConnectAsync();
        }

        private async Task ConnectAsync(string connectionUrl)
        {
            _connectionUrl = connectionUrl;
            await _socket.ConnectAsync(connectionUrl);
        }
    }
}
