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
        }
    }
}
