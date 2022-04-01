// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Rest;
using Discord.API.Rest.Gateway;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordClient
    {
        private IGatewayService? _gatewayService;

        public async Task LoginAsync(string token)
        {
            InitializeServices(token);
            SetupGateway();
        }

        private void InitializeServices(string token)
        {
            DiscordRestFactory restFactory = new DiscordRestFactory();
            restFactory.Token = token;
            _gatewayService = restFactory.GetGatewayService();
        }

        private async void SetupGateway()
        {
            Guard.IsNotNull(_gatewayService, nameof(_gatewayService));

            var gatewayConfig = await _gatewayService.GetGatewayConfig();
        }
    }
}
