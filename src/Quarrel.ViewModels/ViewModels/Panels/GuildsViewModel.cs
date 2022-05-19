// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.GuildSettings;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The view model for the guild list in the app.
    /// </summary>
    public partial class GuildsViewModel : ObservableRecipient
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IMessenger _messenger;
        private readonly ILocalizationService _localizationService;
        private readonly IDiscordService _discordService;
        private readonly IDispatcherService _dispatcherService;
        private readonly ConcurrentDictionary<ulong, BindableGuild> _guilds;

        private IBindableSelectableGuildItem? _selectedGuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildsViewModel"/> class.
        /// </summary>
        public GuildsViewModel(IAnalyticsService analyticsService, IMessenger messenger, ILocalizationService localizationService, IDiscordService discordService, IDispatcherService dispatcherService)
        {
            _analyticsService = analyticsService;
            _messenger = messenger;
            _localizationService = localizationService;
            _discordService = discordService;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<IBindableGuildListItem>();
            _guilds = new ConcurrentDictionary<ulong, BindableGuild>();

            OpenGuildSettingsCommand = new RelayCommand(OpenGuildSettings);

            _messenger.Register<UserLoggedInMessage>(this, (_, _) => LoadGuilds());
            _messenger.Register<NavigateToGuildMessage<ulong>>(this, (_, m) => ForwardNavigate(m.Guild));
        }

        /// <summary>
        /// Gets or sets the selected guild.
        /// </summary>
        public IBindableSelectableGuildItem? SelectedGuild
        {
            get => _selectedGuild;
            set
            {
                if (_selectedGuild is not null)
                    _selectedGuild.IsSelected = false;

                if (SetProperty(ref _selectedGuild, value) && value is not null)
                {
                    value.IsSelected = true;
                    _analyticsService.Log(LoggedEvent.GuildOpened);
                    _messenger.Send(new NavigateToGuildMessage<IBindableSelectableGuildItem>(value));
                }
            }
        }

        /// <summary>
        /// Gets the guild folders.
        /// </summary>
        public ObservableCollection<IBindableGuildListItem> Source { get; private set; }

        /// <summary>
        /// Gets a command that opens the guild settings page for this guild
        /// </summary>
        public RelayCommand OpenGuildSettingsCommand { get; }

        /// <summary>
        /// Loads the guilds for the user.
        /// </summary>
        public void LoadGuilds()
        {
            var folders = _discordService.GetMyGuildFolders();
            _dispatcherService.RunOnUIThread(() =>
            {
                Source.Clear();
                Source.Add(new BindableHomeItem(_messenger, _discordService, _dispatcherService, _localizationService));
                foreach (var folder in folders)
                {
                    if (folder.Folder.Id is null)
                    {
                        foreach (var child in folder.Children)
                        {
                            _guilds.TryAdd(child.Guild.Id, child);
                            Source.Add(child);
                        }
                    }
                    else
                    {
                        Source.Add(folder);
                    }
                }
                _messenger.Send(new GuildsLoadedMessage());
            });
        }

        private void OpenGuildSettings()
        {
            _messenger.Send(new NavigateToSubPageMessage(typeof(GuildSettingsPageViewModel)));
        }

        private void ForwardNavigate(ulong guildId)
        {
            BindableGuild? guild = GetBindableGuild(guildId);
            if (guild is not null)
            {
                SelectedGuild = guild;
            }
        }

        private BindableGuild? GetBindableGuild(ulong guildId)
        {
            if (_guilds.TryGetValue(guildId, out var guild))
            {
                return guild;
            }

            return null;
        }
    }
}
