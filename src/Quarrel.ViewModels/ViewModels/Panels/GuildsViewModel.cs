// Adam Dernis © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Guilds;
using Quarrel.Messages.Discord;
using Quarrel.Messages.Navigation;
using Quarrel.Services.Discord;
using Quarrel.Services.DispatcherService;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels
{
    public partial class GuildsViewModel : ObservableObject
    {
        private readonly IMessenger _messenger;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;

        private BindableGuild _selectedGuild;

        public GuildsViewModel(IMessenger messenger, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _messenger = messenger;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<BindableGuild>();

            _messenger.Register<UserLoggedInMessage>(this, (_, _) => LoadGuilds());
        }

        public BindableGuild SelectedGuild
        {
            get => _selectedGuild;
            set
            {
                if (_selectedGuild is not null)
                    _selectedGuild.IsSelected = false;

                if (SetProperty(ref _selectedGuild, value) && value is not null)
                    value.IsSelected = true;
            }
        }

        public ObservableCollection<BindableGuild> Source { get; private set; }

        public void LoadGuilds()
        {
            var guilds = _discordService.GetMyGuilds();
            _dispatcherService.RunOnUIThread(() =>
            {
                foreach (var guild in guilds)
                {
                    Source.Add(guild);
                }
            });
        }

        public void NavigateToGuild(BindableGuild? guild)
        {
            if (guild is not null)
            {
                _messenger.Send(new NavigateToGuildMessage<BindableGuild>(guild));
            }
        }
    }
}
