// Adam Dernis © 2022

using CommunityToolkit.Diagnostics;
using Discord.API.Gateways;
using Discord.API.Gateways.Models.Handshake;

namespace Discord.API
{
    public partial class DiscordClient
    {
        private void OnReady(object sender, GatewayEventArgs<Ready> e)
        {
            Guard.IsNotNull(e.EventData, nameof(e.EventData));
            Ready ready = e.EventData;

            AddSelfUser(ready.User);

            foreach (var guild in ready.Guilds)
            {
                // All child members are handled here
                AddGuild(guild);
            }
        }
    }
}
