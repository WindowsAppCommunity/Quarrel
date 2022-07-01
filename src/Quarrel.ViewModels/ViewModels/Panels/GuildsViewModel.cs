// Quarrel © 2022

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Quarrel.Bindables.Guilds;
using Quarrel.Bindables.Guilds.Interfaces;
using Quarrel.Client;
using Quarrel.Client.Models.Settings;
using Quarrel.Messages;
using Quarrel.Messages.Navigation;
using Quarrel.Messages.Navigation.SubPages;
using Quarrel.Services.Analytics;
using Quarrel.Services.Analytics.Enums;
using Quarrel.Services.Clipboard;
using Quarrel.Services.Discord;
using Quarrel.Services.Dispatcher;
using Quarrel.Services.Localization;
using Quarrel.ViewModels.SubPages.Settings.GuildSettings;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Quarrel.ViewModels
{
    /// <summary>
    /// The view model for the guild list in the app.
    /// </summary>
    public partial class GuildsViewModel : ObservableRecipient
    {
        private readonly ILoggingService _loggingService;
        private readonly IMessenger _messenger;
        private readonly ILocalizationService _localizationService;
        private readonly IClipboardService _clipboardService;
        private readonly IDiscordService _discordService;
        private readonly QuarrelClient _quarrelClient;
        private readonly IDispatcherService _dispatcherService;
        private readonly ConcurrentDictionary<ulong, BindableGuild> _guilds;

        private IBindableSelectableGuildItem? _selectedGuild;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildsViewModel"/> class.
        /// </summary>
        public GuildsViewModel(
            ILoggingService loggingService,
            IMessenger messenger,
            ILocalizationService localizationService,
            IClipboardService clipboardService,
            IDiscordService discordService,
            QuarrelClient quarrelClient,
            IDispatcherService dispatcherService)
        {
            _loggingService = loggingService;
            _messenger = messenger;
            _localizationService = localizationService;
            _clipboardService = clipboardService;
            _discordService = discordService;
            _quarrelClient = quarrelClient;
            _dispatcherService = dispatcherService;

            Source = new ObservableCollection<IBindableGuildListItem>();
            _guilds = new ConcurrentDictionary<ulong, BindableGuild>();

            OpenGuildSettingsCommand = new RelayCommand(OpenGuildSettings);

            _messenger.Register<UserLoggedInMessage>(this, (_, _) => LoadGuilds());
            _messenger.Register<SelectGuildMessage<ulong>>(this, (_, m) => SelectGuildById(m.Guild));
            _messenger.Register<SelectGuildMessage<IBindableSelectableGuildItem>>(this, (_, m) => SelectedGuild = m.Guild);
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
                    _loggingService.Log(LoggedEvent.GuildOpened);
                    _messenger.Send(new GuildSelectedMessage<IBindableSelectableGuildItem>(value));
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
        /// Gets the current user's guild folders with children.
        /// </summary>
        /// <remarks>
        /// Contains null folders, whose children should be treated as though they're in the roots.
        /// </remarks>
        /// <returns>The array of <see cref="BindableGuildFolder"/>s that the current user has.</returns>
        public BindableGuildFolder[] GetMyGuildFolders()
        {
            GuildFolder[] rawFolders = _quarrelClient.Guilds.GetMyGuildFolders();
            BindableGuildFolder[] folders = new BindableGuildFolder[rawFolders.Length];
            for (int i = 0; i < rawFolders.Length; i++)
            {
                folders[i] = new BindableGuildFolder(_messenger, _discordService, _quarrelClient, _dispatcherService, _clipboardService, _localizationService, rawFolders[i]);
            }

            return folders;
        }

        /// <summary>
        /// Loads the guilds for the user.
        /// </summary>
        public void LoadGuilds()
        {
            var folders = GetMyGuildFolders();
            _dispatcherService.RunOnUIThread(() =>
            {
                Source.Clear();
                Source.Add(new BindableHomeItem(_messenger, _discordService, _quarrelClient, _dispatcherService, _localizationService));
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
            if (SelectedGuild is BindableGuild guild)
            {
                var viewModel = new GuildSettingsPageViewModel(_localizationService, _discordService, guild);
                _messenger.Send(new NavigateToSubPageMessage(viewModel));
            }
        }

        private void SelectGuildById(ulong guildId)
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
