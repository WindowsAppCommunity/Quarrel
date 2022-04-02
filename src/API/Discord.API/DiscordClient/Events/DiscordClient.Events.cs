// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;

namespace Discord.API
{
    public partial class DiscordClient
    {
        private void RegisterEvents()
        {
            Guard.IsNotNull(_gateway, nameof(_gateway));

            _gateway.Ready += OnReady;
            _gateway.MessageCreated += _gateway_MessageCreated;
        }

        private void _gateway_MessageCreated(object sender, Gateways.GatewayEventArgs<Models.Json.Messages.JsonMessage> e)
        {
            throw new System.NotImplementedException();
        }
    }
}
