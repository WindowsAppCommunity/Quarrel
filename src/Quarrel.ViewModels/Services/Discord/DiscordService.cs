// Adam Dernis © 2022

using Discord.API;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public class DiscordService : IDiscordService
    {
        private DiscordClient _discordClient;
        private IMessenger _messenger;

        public DiscordService(IMessenger messenger)
        {
            _messenger = messenger;
            _discordClient = new DiscordClient();
        }

        /// <inheritdoc/>
        public async Task LoginAsync(string token)
        {
            await _discordClient.LoginAsync(token);
        }
    }
}
