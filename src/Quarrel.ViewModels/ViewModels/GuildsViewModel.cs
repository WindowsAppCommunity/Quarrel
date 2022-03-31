// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Services.Discord;

namespace Quarrel.ViewModels
{
    public class GuildsViewModel
    {
        private readonly IDiscordService _discordService;
        private readonly IMessenger _messenger;

        public GuildsViewModel(IDiscordService discordService, IMessenger messenger)
        {
            _discordService = discordService;
            _messenger = messenger;
        }
    }
}
