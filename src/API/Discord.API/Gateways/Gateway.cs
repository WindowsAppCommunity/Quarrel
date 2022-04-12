// Adam Dernis © 2022

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

        public async Task<bool> ConnectAsync()
        {
            _gatewayStatus = GatewayStatus.Connecting;
            return await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "9", "&compress=zlib-stream"));
        }

        public async Task<bool> ResumeAsync()
        {
            _gatewayStatus = GatewayStatus.Resuming;
            _socket = CreateSocket();
            return await ConnectAsync();
        }

        private async Task<bool> ConnectAsync(string connectionUrl)
        {
            try
            {
                _connectionUrl = connectionUrl;
                await _socket.ConnectAsync(connectionUrl);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
