// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Rest;
using Discord.API.Rest.Gateway;
using System.Threading.Tasks;

namespace Discord.API
{
    public partial class DiscordClient
    {
        private IGatewayService? _gatewayService;
        private Gateway? _gateway;
        private string? _token;

        public string? Token => _token;

        public async Task LoginAsync(string token)
        {
            _token = token;
            InitializeServices(token);
            await SetupGatewayAsync(token);
        }

        private void InitializeServices(string token)
        {
            DiscordRestFactory restFactory = new DiscordRestFactory();
            restFactory.Token = token;
            _gatewayService = restFactory.GetGatewayService();
        }

        private async Task SetupGatewayAsync(string token)
        {
            Guard.IsNotNull(_gatewayService, nameof(_gatewayService));

            var gatewayConfig = await _gatewayService.GetGatewayConfig();
            _gateway = new Gateway(gatewayConfig, token);
            await _gateway.ConnectAsync();
            RegisterEvents();
        }
    }
}
