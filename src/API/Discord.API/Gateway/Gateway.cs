// Adam Dernis © 2022

using Discord.API.Models.Json.Gateway;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Discord.API.Gateway
{
    internal partial class Gateway
    {
        private delegate void GatewayEventHandler(SocketFrame gatewayEvent);

        private readonly GatewayConfig _gatewayConfig;
        private readonly string _token;

        private IReadOnlyDictionary<OperationCode, GatewayEventHandler> _operationHandlers;
        private IReadOnlyDictionary<string, GatewayEventHandler> _eventHandlers;

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

            _operationHandlers = GetOperationHandlers();
            _eventHandlers  = GetEventHandlers();

            _gatewayStatus = GatewayStatus.Initialized;
        }

        public async Task<bool> ConnectAsync()
        {
            string append = string.Empty;
            append = "&compress=zlib-stream";

            return await ConnectAsync(_gatewayConfig.GetFullGatewayUrl("json", "6", append));
        }

        public async Task<bool> ResumeAsync()
        {
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
