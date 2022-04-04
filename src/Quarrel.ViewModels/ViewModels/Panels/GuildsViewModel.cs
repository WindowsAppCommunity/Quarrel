// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Messages.Discord;
using Quarrel.Models.Bindables;
using Quarrel.Services.Discord;
using Quarrel.Services.DispatcherService;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels
{
    public class GuildsViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        public GuildsViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<BindableGuild>();

            _messenger.Register<UserLoggedInMessage>(this, (_, _) => _dispatcherService.RunOnUIThread(LoadGuildsAsync));
        }

        public ObservableCollection<BindableGuild> Source { get; set; }

        public async void LoadGuildsAsync()
        {
            var guilds = _discordService.GetMyGuilds();
            foreach (var guild in guilds)
            {
                Source.Add(guild);
            }
        }
    }
}
