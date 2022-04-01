// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Gateways.Models.Handshake;
using Discord.API.Rest;
using Discord.API.Rest.Gateway;
using System;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordClient
    {
        private IGatewayService? _gatewayService;
        private Gateway? _gateway;

        public async Task LoginAsync(string token)
        {
            InitializeServices(token);
            SetupGateway(token);
        }

        private void InitializeServices(string token)
        {
            DiscordRestFactory restFactory = new DiscordRestFactory();
            restFactory.Token = token;
            _gatewayService = restFactory.GetGatewayService();
        }

        private async void SetupGateway(string token)
        {
            Guard.IsNotNull(_gatewayService, nameof(_gatewayService));

            var gatewayConfig = await _gatewayService.GetGatewayConfig();
            _gateway = new Gateway(gatewayConfig, token);
            await _gateway.ConnectAsync();
            _gateway.Ready += OnReady;
        }

        private void OnReady(object sender, GatewayEventArgs<Ready> e)
        {
            throw new NotImplementedException();
        }
    }
}
