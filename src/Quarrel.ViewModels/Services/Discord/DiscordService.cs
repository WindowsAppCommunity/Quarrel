// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace Quarrel.Services.Discord
{
    public class DiscordService : IDiscordService
    {
        private IMessenger _messenger;

        public DiscordService(IMessenger messenger)
        {
            _messenger = messenger;
        }

        /// <inheritdoc/>
        public async Task Login(string token)
        {

        }
    }
}
